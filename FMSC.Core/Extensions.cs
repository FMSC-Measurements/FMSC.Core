using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace FMSC.Core
{
    public static class Extensions
    {
        #region Bit Extensions
        public static bool GetBit(this int value, byte bitPosition)
        {
            return (value & (1 << bitPosition)) != 0;
        }

        public static void SetBit(this ref int value, bool bitSet, byte bitPosition)
        {
            if (bitSet)
                value |= 1 << bitPosition;
            else
                value &= ~(1 << bitPosition);
        }
        #endregion

        #region List Extensions
        public static bool Matches<T>(this IList<T> list, IList<T> compareList, bool InOrder = false)
        {
            if (InOrder)
            {
                if (list.Count != compareList.Count)
                    return false;

                for (int i = 0; i < list.Count; i++)
                {
                    if (!list[i].Equals(compareList[i]))
                        return false;
                }

                return true;
            }
            else
            {
                return list.ContainsOnly(compareList);
            }
        }


        public static string ToStringContents<T>(this IList<T> list, string delim = " ")
        {
            StringBuilder sb = new StringBuilder();

            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count - 1; i++)
                {
                    sb.AppendFormat("{0}{1}", list[i], delim);
                }

                sb.Append(list[list.Count - 1]);
            }

            return sb.ToString();
        }

        public static string ToStringContents<T>(this IList<T> list, Func<T, string> func, string delim = " ")
        {
            StringBuilder sb = new StringBuilder();

            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count - 1; i++)
                {
                    sb.AppendFormat("{0}{1}", func(list[i]), delim);
                }

                sb.Append(func(list[list.Count - 1]));
            }

            return sb.ToString();
        }
        #endregion

        #region Dictionary Extensions
        public static bool DictionaryEqual<TKey, TValue>(
            this IDictionary<TKey, TValue> dic1,
            IDictionary<TKey, TValue> dic2,
            IEqualityComparer<TValue> valueComparer = null)
        {
            if (dic1 == dic2)
                return true;

            if (dic1 == null || dic2 == null || dic1.Count != dic2.Count)
                return false;

            valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;

            TValue value2;
            foreach (var kvp in dic1)
            {
                if (!dic2.TryGetValue(kvp.Key, out value2))
                    return false;

                if (!valueComparer.Equals(kvp.Value, value2))
                    return false;
            }

            return true;
        }
        #endregion

        #region Linq Extensions
        public static bool ContainsAll<T>(this IEnumerable<T> list, IEnumerable<T> compareList)
        {
            foreach (T item in compareList)
            {
                if (!list.Contains(item))
                    return false;
            }

            return true;
        }

        public static bool EqualsNonOrdered<T>(this IEnumerable<T> list, IEnumerable<T> compareList)
        {
            var cnt = new Dictionary<T, int>();

            foreach (T s in list)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }

            foreach (T s in compareList)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }


        public static bool ContainsOnly<T>(this IEnumerable<T> list, IEnumerable<T> compareList)
        {
            if (list.Count() != compareList.Count())
                return false;

            foreach (T item in compareList)
            {
                if (!list.Contains(item))
                    return false;
            }

            return true;
        }


        public static IEnumerable<T> GetFromIndices<T>(this IList<T> list, IEnumerable<int> indices, bool removeDuplicates = true)
        {
            IEnumerable<T> listFromIndices = indices.Select(i => list[i]);
            return removeDuplicates ? listFromIndices.Distinct() : listFromIndices;
        }


        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (T item in list)
            {
                action(item);
            }
        }


        public static bool HasAtLeast<T>(this IEnumerable<T> source, int amount, Func<T, bool> predicate = null)
        {
            if (amount < 1)
                throw new ArgumentException("amount must be at least 1");

            int count = 0;

            if (predicate != null)
            {
                foreach (T item in source)
                {
                    if (predicate(item))
                        count++;

                    if (count == amount)
                        return true;
                }
            }
            else
            {
                foreach (T item in source)
                {
                    count++;

                    if (count == amount)
                        return true;
                }
            }

            return false;
        }
        #endregion

        #region T Extensions
        public static bool IsNullable<T>(this T obj)
        {
            if (obj == null) return true;
            Type type = typeof(T);
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }

        public static string GetDescription<T>(this T obj)
        {
            //Tries to find a DescriptionAttribute for a potential friendly name
            MemberInfo[] memberInfo = obj.GetType().GetMember(obj.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            throw new Exception("No object description");
        }


        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T DeepClone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
                throw new ArgumentException("The type must be serializable.", "source");

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
                return default(T);

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Checks to see if a type implements an interface
        /// </summary>
        /// <typeparam name="T">Type of Interface that Type may implement</typeparam>
        /// <param name="type">Type that is checked against an interface</param>
        /// <returns></returns>
        public static bool ImplementsInterface<T>(this Type type)
        {
            if (!typeof(T).IsInterface) throw new InvalidOperationException();
            return type != null && type.GetInterfaces().Any(x => x == typeof(T));
        }

        public static String ToStringOrEmpty<T>(this T item)
        {
            if (item == null) return String.Empty;
            return item.ToString();
        }
    }
    #endregion
}
