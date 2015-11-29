namespace Sledgehammer.Tests.TestTypes
{
    public static class StaticClass
    {
        public static void WithVoidMethod()
        {
        }

        // ReSharper disable once UnusedParameter.Global
        public static void WithVoidMethodArgs(int x)
        {
        }

        public static int WithStaticIntMethod()
        {
            throw new System.NotImplementedException();
        }

        public static string WithStaticStringMethod()
        {
            throw new System.NotImplementedException();
        }
    }
}