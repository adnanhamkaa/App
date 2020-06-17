using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models {
    public class DttResponseForm<T> where T : new() {
        private List<T> _data;

        public int draw { get; set; }
        public long recordsTotal { get; set; }
        public long recordsFiltered { get; set; }

        public List<T> data {
            get { return _data ?? (_data = new List<T>()); }
            set { _data = value; }
        }
    }

    public class DttResponseErrorForm<T> : DttResponseForm<T> where T : new() {
        public DttResponseErrorForm() { }

        public DttResponseErrorForm(DttResponseForm<T> fm) {
            Initialize(fm);
        }

        public string error { get; set; }

        public void Initialize(DttResponseForm<T> fm) {
            draw = fm.draw;
            recordsTotal = fm.recordsTotal;
            recordsFiltered = fm.recordsFiltered;
            data = fm.data;
        }
    }
}