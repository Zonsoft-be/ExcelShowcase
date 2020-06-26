using Application.Models;
using ServiceStack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Media.Core;

namespace Application.Data
{
    public class Database : IDatabase
    {
        public ConcurrentBag<Identifiable> Identifiables { get; set; } 

        public ConcurrentDictionary<string, Identifiable[]> ObjectsByType { get; set; }

        public Database()
        {
            this.Identifiables = new ConcurrentBag<Identifiable>();

            this.ObjectsByType = new ConcurrentDictionary<string,  Identifiable[]>();

            this.Init();
        }

        public void Init()
        {
            var products = new List<Product>();
           
            var randomQty = new Random(124578);

            foreach (var index in Enumerable.Range(1, 10000))
            {
                var product = (Product)this.Create<Product>(null);

                product.Name = $"Name {index}";
                product.Description = $"Description {index}";
                product.Unit = "Piece";

                var qty = randomQty.Next(10000);
                var price = new decimal(randomQty.Next(0, 1000) * randomQty.NextDouble());

                product.UnitPrice = price;
                product.Quantity = qty;

                products.Add(product);
            }

            this.Store<Product>(products.ToArray());           
        }

        /// <inheritdoc/>       
        public Identifiable Create<T>(Type t, params object[] parameters) where T : Identifiable
        {            
            var instance = (T) Activator.CreateInstance(typeof(T), parameters);
            Identifiables.Add(instance);
                        
            instance.Id = -1;

            return instance;
        }

        public void Store<T>(T[] array) where T : Identifiable
        {
            if (ObjectsByType.ContainsKey(typeof(T).Name))
            {
                ObjectsByType[typeof(T).Name] = array;
            }
            else
            {
                ObjectsByType.TryAdd(typeof(T).Name, array);
            }
        }

        public void Store<T>(T instance) where T : Identifiable
        {
            if (ObjectsByType.ContainsKey(typeof(T).Name))
            {
                var array = ObjectsByType[typeof(T).Name];
                Array.Resize(ref array, array.Length + 1);
                array[array.Length -1] = instance;
                ObjectsByType[typeof(T).Name] = array;
            }
            else
            {
                ObjectsByType.TryAdd(typeof(T).Name, new T[1] { instance });
            }           
        }


        public T[] Get<T>() where T : Identifiable
        {
            if (ObjectsByType.ContainsKey(typeof(T).Name))
            {
                return ObjectsByType[typeof(T).Name].Cast<T>().ToArray();
            }
            else
            {
                return Array.Empty<T>();
            }
        }

        public void Save<T>(T instance) where T : Identifiable
        {
            if (instance.Id == -1)
            {
                instance.Id = this.Get<T>().Length + 1;      
                this.Store<T>(instance);
            }

            instance.OnSave(this);
        }

        public int Count<T>() where T : Identifiable
        {
            if (ObjectsByType.ContainsKey(typeof(T).Name))
            {
                return ObjectsByType[typeof(T).Name].Count();
            }
            else
            {
                return 0;
            }
        }

        public T FirstOrDefault<T>(Func<T, bool> func) where T : Identifiable
        {
            if (ObjectsByType.ContainsKey(typeof(T).Name))
            {             
                return ((T[])ObjectsByType[typeof(T).Name]).FirstOrDefault<T>(func);
            }
            else
            {
                return null;
            }            
        }
    }
}
