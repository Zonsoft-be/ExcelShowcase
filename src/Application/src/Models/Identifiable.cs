using Application.Data;
using System;

namespace Application.Models
{
    public class Identifiable
    {
        public long Id { get; set; }

        /// <summary>
        /// OnSave is run for newly created objects (id=-1) before the instance is stored.
        /// </summary>
        public virtual void OnSave(IDatabase database)
        {

        }
    }
}