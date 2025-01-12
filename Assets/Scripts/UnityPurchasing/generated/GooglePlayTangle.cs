// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("cboS3DR2pZRs2EsOxO/uxDyFQU6tBVohuZEGxzhNpEbHYgkcdWO7lFKnlI3BTAZRNYmbBjPA+cB6GFqsdXZiiJtDg3OoJ+DY7Of9AZjp7PXNKpLzxPRgqUDB3O4PutcASpUN9vzEVhzPZ5HgiCY1oODOzGZyENCMMIWVx3ie9uzBjAzgXzgrwegyBA6bPeKbaGTBaAz2RtIoB31KogAj9Bhv/RJ58f+CBG+rJkzj4CZpyo/6d3SpAlY16hQTjYxq4bAohGm5am0RoyADESwnKAunaafWLCAgICQhIsy5SnOUg9qsQGZ8yIRrxiTGBeG66XxZclezn2UYFPc3h+sCuAmSTMujIC4hEaMgKyOjICAh+68tOuk0riNMNsnhvlUkDCMiICEg");
        private static int[] order = new int[] { 8,4,10,13,12,9,9,9,9,13,13,11,13,13,14 };
        private static int key = 33;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
