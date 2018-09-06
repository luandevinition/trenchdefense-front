#!/usr/bin/env bash

PROTOCOL_HOME=${1%/}

export PATH=/Library/Frameworks/Mono.framework/Versions/Current/bin/:${PATH}


protolist=$(find $PROTOCOL_HOME -name "*.proto")
modellist=()
for filepath in $protolist; do
  model=$(basename $filepath .proto)
  echo "Start compiling proto... $model"
  modellist+=($model)
  protoc --descriptor_set_out=$model.pb --proto_path=$PROTOCOL_HOME $filepath
  mono protobuf-net/ProtoGen/protogen.exe -i:$model.pb -o:$model.pb.cs
done

gmcs -target:library -langversion:ISO-2 -r:Assets/Plugins/protobuf-net.dll -out:Assets/Plugins/proto.dll $(find . -name "*.pb.cs")
mono protobuf-net/Precompile/precompile.exe Assets/Plugins/proto.dll -o:Assets/Plugins/proto_serializer.dll -t:ProtoSerializer Assets/Plugins/protobuf-net.dll

rm -fr *.pb
rm -fr *.pb.cs

mcs Assets/Plugins/Editor/TypeTableBuild/TypeTableUtilBuilder.cs
mono Assets/Plugins/Editor/TypeTableBuild/TypeTableUtilBuilder.exe ${modellist[@]} > Assets/Scripts/Utils/TypeTableUtil.cs
