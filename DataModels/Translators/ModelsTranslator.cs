using System.Reflection;
using Wattmate_Site.DataModels.Attributes;
using Wattmate_Site.DataModels.DataTransferModels;

namespace Wattmate_Site.DataModels.Translators
{
    public static class ModelsTranslator
    {
        /// <summary>
        /// Translates a List of T data models into a list of T data transfer objects
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="sourceList"></param>
        /// <returns></returns>
        public static List<TTarget> ListToDTO<TSource, TTarget>(this IEnumerable<TSource> sourceList)
       where TTarget : new()
        {
            return sourceList.Select(item => ToDTO<TSource, TTarget>(item)).ToList();
        }

        /// <summary>
        /// Translate a data model T into a data transfer object T
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TTarget ToDTO<TSource, TTarget>(TSource source)
               where TTarget : new()
        {
            if (source == null) return default;

            // Get the properties of the source
            var sourceProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Get the properties of target
            var targetProps = typeof(TTarget).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var target = new TTarget();

            // Go through all the properties of the source, and try to get a match in the target
            foreach (var sourceProp in sourceProps)
            {
                // Skip properties marked with [HideFromDTO]
                if (Attribute.IsDefined(sourceProp, typeof(HideFromDTOAttribute)))
                    continue;

                // Try to find a match in the target
                var targetProp = targetProps.FirstOrDefault(p => p.Name == sourceProp.Name &&
                                                                 p.PropertyType == sourceProp.PropertyType &&
                                                                 p.CanWrite);

                // If found, copy the value
                if (targetProp != null)
                {
                    var value = sourceProp.GetValue(source);
                    targetProp.SetValue(target, value);
                }
            }

            return target;
        }
    }
}
