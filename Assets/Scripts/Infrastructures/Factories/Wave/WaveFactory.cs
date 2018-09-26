namespace Infrastructures.Factories.Wave
{
	public static class WaveFactory {

		public static Domain.Wave Make(App.Proto.RequestAccessTokenParameter dto)
		{
			return new Domain.Wave();
		}
	}
}
