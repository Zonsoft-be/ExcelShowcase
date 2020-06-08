using Application.Models;

namespace Application.Data
{
    public interface IDatabase
    {
        Identifiable Create<T>() where T : Identifiable;
        
        void Store<T>(T[] array) where T : Identifiable;

        void Store<T>(T instance) where T : Identifiable;

        T[] Get<T>() where T : Identifiable;

        void Save<T>(T instance) where T : Identifiable;
    }
}