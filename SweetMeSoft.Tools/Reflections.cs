using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SweetMeSoft.Tools
{
    public class Reflections
    {
        public static void CleanVirtualPropertiesList<T>(IEnumerable<T> entities, List<string> originalNames = null)
        {
            foreach (var entity in entities)
            {
                CleanVirtualProperties(entity, originalNames);
            }
        }

        /// <summary>
        /// Método para limpiar propiedades virtuales de entidades de BD para evitar referencias circulares.
        /// </summary>
        /// <typeparam name="T">Tipo de la entidad de Base de datos</typeparam>
        /// <param name="entity">Entidad de Base de datos a limpiar</param>
        /// <param name="originalNames">Lista de nombres de propiedades a los que ya se les borró las referencias circulares.Inicialmente debe recibir una lista con un solo valor que será el nombre de la Entidad T. Luego esta lista se renna recursivamente.</param>
        public static void CleanVirtualProperties<T>(T entity, List<string> originalNames = null)
        {
            originalNames ??= new List<string>
                {
                    typeof(T).Name
                };

            if (entity != null)
            {
                var copyNames = new List<string>();
                copyNames.AddRange(originalNames);
                PropertyInfo[] properties = typeof(T).GetProperties().Where(p => p.GetGetMethod().IsVirtual).ToArray();
                foreach (PropertyInfo property in properties)
                {
                    if (property.PropertyType.IsGenericType
                        && (property.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
                        || property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)
                        || property.PropertyType.GetGenericTypeDefinition() == typeof(IList<>)
                        || property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                    {
                        if (property.GetValue(entity, null) != null)
                        {
                            dynamic obj = property.GetValue(entity, null);
                            var items = Enumerable.ToList(obj);

                            if (items.Count > 0 && !copyNames.Contains(items[0].GetType().Name))
                            {
                                copyNames.Add(items[0].GetType().Name);
                                foreach (var item in items)
                                {
                                    CleanVirtualProperties(item, copyNames);
                                }
                            }
                            else
                            {
                                if (items.Count > 0)
                                {
                                    property.SetValue(entity, null);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!copyNames.Contains(property.Name) && property.GetValue(entity, null) != null)
                        {
                            copyNames.Add(property.Name);
                            dynamic obj = property.GetValue(entity, null);
                            CleanVirtualProperties(obj, copyNames);
                        }
                        else
                        {
                            property.SetValue(entity, null);
                        }
                    }
                }
            }
        }
    }
}
