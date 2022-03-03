using System;
using System.Collections.Generic;


namespace BaltaDataAccess.Models
{
    public class Career
    {
        public Career()
        {
            Items = new List<CareerItem>(); //Aqui já inicializamos a lista para garantir que não teremos problema com objeto nulo. 
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public IList<CareerItem> Items { get; set; }
    }
}
