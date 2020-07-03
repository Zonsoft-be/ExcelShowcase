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

            this.ObjectsByType = new ConcurrentDictionary<string, Identifiable[]>();

            this.Init();
        }

        public void Init()
        {
            var products = new Product[100000];

            var randomQty = new Random(124578);

            for (int index = 0; index < products.Length; index++)
            {
                var product = (Product)this.Create<Product>(null);

                product.Name = $"Name {index}";
                product.Description = $"Description {index}";
                product.Unit = "Piece";

                var qty = randomQty.Next(10000);
                var price = new decimal(randomQty.Next(0, 1000) * randomQty.NextDouble());

                product.UnitPrice = price;
                product.Quantity = qty;

                products[index] = product;
            }


            this.Store<Product>(products);

            var paymentTerms = new PaymentTerm[2]
            {
                    new PaymentTerm("INV", 30, false, "INV30 (Betaling 30 dagen na factuurdatum.)"),
                    new PaymentTerm("EOM", 30, true, "EOP30 (Betaling 30 dagen na einde maand factuurdatum.)")
            };

            this.Store<PaymentTerm>(paymentTerms);

            var organisations = new Organisation[3]
            {
                  new Organisation()
                    {
                        Name = "Zonsoft.be",
                        Street = "Uikhoverstraat 158",
                        City = "BE 3631 Maasmechelen",
                        Country = "Belgium",
                        VatNumber = "BE 0880.592.625",
                        FinancialContact = "Walter Hesius"
                    },
                    new Organisation()
                    {
                        Name = "Dipu",
                        Street = "Kleine NieuweDijkstraat 2",
                        City = "BE 2600 Mechelen",
                        Country = "Belgium",
                        VatNumber = "BE 0880.592.625",
                        FinancialContact = "Koen van Exem",
                        DefaultPaymentTerm = paymentTerms[0]
                    },
                    new Organisation()
                    {
                        Name = "Aperam SA",
                        Street = "12C rue Guillaume Kroll",
                        City = "L-1882 Luxembourg",
                        Country = "Luxemburg",
                        VatNumber = "B 155908",
                        FinancialContact = "Alexis GOUDRIAS,",
                        DefaultPaymentTerm = paymentTerms[1]
                    },
            };

            this.Store<Organisation>(organisations);


        }

        /// <inheritdoc/>       
        public Identifiable Create<T>(Type t, params object[] parameters) where T : Identifiable
        {
            var instance = (T)Activator.CreateInstance(typeof(T), parameters);
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
                array[array.Length - 1] = instance;
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

        public void Save<T>(T[] instances) where T : Identifiable
        {
            foreach (var instance in instances)
            {
                this.Save(instance);
            }           
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
                return ObjectsByType[typeof(T).Name]
                    .Cast<T>()
                    .FirstOrDefault(func);
            }
            else
            {
                return null;
            }
        }
    }
}
