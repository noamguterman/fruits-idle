#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("fU6X+kvOLXoKMz31t09d+Muga6IgQ3AZu4jZbZTdcYxiF9mQ4VSzqPmq/wtIhPqPSzKzjDVLUP9g7ZEtWtnX2Oha2dLaWtnZ2CukmuneiwnQiStqIfVepEjWmzND9mIODhEhAq6IQ3wVBKti2zgxHp7pkOvNnL2YmZnhZ+VzZcXHCQWyJ9JKXBKdl1Nwaps9fWsns4uVAaf6ToyS7038Jl4Lb0Fb7mTtC37gvNTmecr8vcUIMdUYCljmakDsbO41UL8HAR/gf/qwhjb3FcmZeJ33XHOwf+rFfphewOha2fro1d7R8l6QXi/V2dnZ3djbcjWWbQJwY2DEsQqrproUhbjf8FxqAnb781CfxkXmDoqsMETGkRrhDpYeVoyv3HSo4drb2djZ");
        private static int[] order = new int[] { 7,2,11,11,12,10,9,10,8,12,11,11,13,13,14 };
        private static int key = 216;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
