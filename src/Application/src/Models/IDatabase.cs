using Application.Models;
using System;

namespace Application.Data
{
    public interface IDatabase
    {
        /// <summary>
        /// Create an object of Type T, with Constructor parameters 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Identifiable Create<T>(Type t, params object [] parameters) where T : Identifiable;
        
        void Store<T>(T[] array) where T : Identifiable;

        void Store<T>(T instance) where T : Identifiable;

        T[] Get<T>() where T : Identifiable;

        int Count<T>() where T : Identifiable;

        void Save<T>(T instance) where T : Identifiable;
    }
}