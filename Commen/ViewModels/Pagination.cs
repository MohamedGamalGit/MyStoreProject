using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commen.ViewModels
{
    public class Pagination<T> // Fix for CS0246: Make the class generic to define 'T'
    {
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; } = new List<T>(); // Fix for CS8618: Initialize the property to avoid null values
    }
}
