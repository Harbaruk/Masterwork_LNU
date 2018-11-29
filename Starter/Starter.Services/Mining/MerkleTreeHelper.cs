using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Starter.Services.Mining
{
    public static class MerkleTree<T> where T : class
    {
        public static string Compute(IList<T> collection)
        {
            IList<string> hashed = collection.Select(x => Hash(x)).ToList();

            return HashPairs(hashed);
        }

        private static string Hash(T obj)
        {
            using (SHA256Managed sha = new SHA256Managed())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, obj);

                    return Convert.ToBase64String(sha.ComputeHash(ms.ToArray())).ToLower();
                }
            }
        }

        private static string Hash(string obj)
        {
            using (SHA256Managed sha = new SHA256Managed())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, obj);

                    return Convert.ToBase64String(sha.ComputeHash(ms.ToArray())).ToLower();
                }
            }
        }

        private static string HashPairs(IList<string> collection)
        {
            if (collection.Count() == 1)
            {
                return collection.First();
            }

            IList<string> result = new List<string>();

            for (int i = 0; i < collection.Count(); i += 2)
            {
                if (i + 1 >= collection.Count())
                {
                    result.Add(collection[i]);
                }
                else
                {
                    result.Add(Hash(collection[i] + collection[i + 1]));
                }
            }

            if (result.Count > 1)
            {
                return HashPairs(result);
            }

            return result.First();
        }
    }
}