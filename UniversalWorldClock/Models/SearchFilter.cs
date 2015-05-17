using System;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Mvvm;
using UniversalWorldClock.Domain;

namespace UniversalWorldClock.Models
{
    public sealed class SearchFilter : BindableBase
    {
        private bool _active;
        private string _name;
        private int _count;

        public SearchFilter(String name, int count, bool active = false)
        {
            this.Name = name;
            this.Count = count;
            this.Active = active;
        }

        public override String ToString()
        {
            return Description;
        }

        public String Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public int Count
        {
            get { return _count; }
            set { SetProperty(ref _count, value); }
        }

        public bool Active
        {
            get { return _active; }
            set { SetProperty(ref _active, value); }
        }

        public String Description
        {
            get { return String.Format("{0} ({1})", Name, Count); }
        }


        public List<CityInfo> Cities { get; set; }
    }
}