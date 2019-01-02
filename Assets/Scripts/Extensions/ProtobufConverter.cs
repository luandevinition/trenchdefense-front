using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Proto;
using ProtoBuf;
using UnityEngine;
using Utils;

namespace Extensions
{
    public static class ProtobufConverter
    {
        /// <summary>
        /// モデルのシリアライザー
        /// </summary>
        private static readonly ProtoSerializer Serializer = new ProtoSerializer ();

        /// <summary>
        /// バックエンドからのレスポンスをモデルのリストにデシリアライズする
        /// </summary>
        /// <returns>モデルのリスト</returns>
        /// <param name="bytes">バックエンドから出力されたprotobufでシリアライズされたバイナリ</param>
        public static List<IExtensible> DeserializeResponseData(byte[] bytes)
        {
            var messages = DeserializeProtobufMessagesBinary (bytes);
            return messages.messages.Select(message => DeserializeProtobufMessageToModel(message)).ToList();
        }

        private static ProtobufMessages DeserializeProtobufMessagesBinary(byte[] bytes)
        {
            Debug.LogError("BYTE Size : " + bytes.Length);
            
            var ms = new MemoryStream(bytes);
            var messages = new ProtobufMessages ();
            Serializer.Deserialize (ms, messages, typeof(ProtobufMessages));
            ms.Dispose ();
            return messages;
        }

        /// <summary>
        /// ProtobufMessageに格納されているprotobufModelを取りだいて、Deserializeして返す
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static IExtensible DeserializeProtobufMessageToModel(ProtobufMessage message)
        {
            var model = TypeTableUtil.TypeIdToObject (message.type);

            var ms = new MemoryStream (message.payload);

            try
            {
                Serializer.Deserialize(ms, model, model.GetType());
            }
            catch (Exception ex)
            {
                Debug.LogError("不正なprotoBufモデルです。バックエンドと不整合が発生しています。 Model: " + model.ToString());
                throw ex;
            }
            finally
            {
                ms.Dispose ();
            }

            return model;
        }

        /// <summary>
        /// バックエンドに渡すモデルをシリアライズしてバイナリにする
        /// </summary>
        /// <returns>シリアライズされたバイナリ</returns>
        /// <param name="models">モデルのリスト</param>
        public static byte[] SerializeModels(List<IExtensible> models)
        {
            var messages = new ProtobufMessages {count = models.Count};

            foreach (var model in models) {
                var message = SerializeModelToProtobufMessage (model);
                messages.messages.Add (message);
            }

            return SerializeCommunicationMeesagesToBinary (messages);
        }

        private static ProtobufMessage SerializeModelToProtobufMessage(IExtensible model)
        {
            var message = new ProtobufMessage
            {
                type = TypeTableUtil.ObjectToTypeId(model),
                payload = SerializeModelToBinary(model)
            };
            return message;
        }

        private static byte[] SerializeModelToBinary(IExtensible model)
        {
            var ms = new MemoryStream ();
            Serializer.Serialize (ms, model);
            ms.Seek (0, SeekOrigin.Begin);
            var binary = ms.ToArray ();
            ms.Dispose ();
            return binary;
        }

        private static byte[] SerializeCommunicationMeesagesToBinary(ProtobufMessages messages)
        {
            var ms = new MemoryStream ();
            Serializer.Serialize (ms, messages);
            ms.Seek (0, SeekOrigin.Begin);
            var binary = ms.ToArray ();
            ms.Dispose ();
            return binary;
        }
    }
}