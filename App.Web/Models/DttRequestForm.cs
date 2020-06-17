using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models {
    public class DttRequestForm {

        private List<DttRequestColumnForm> _columns;
        private int? _length;
        private List<DttRequestOrderForm> _order;
        private DttRequestSearchForm _search;
        private int _start;
        private bool? _status;

        public int draw { get; set; }
        public int start {
            get {
                if (_start < 0) {
                    _start = 0;
                }

                return _start;
            }
            set { _start = value; }
        }
        public int? length {
            get {
                if (_length == -1) {
                    _length = int.MaxValue;
                } else if (_length < 1 && _length != -1) {
                    _length = 10;
                }

                return _length;
            }
            set { _length = value; }
        }

        public DttRequestSearchForm search {
            get { return _search ?? (_search = new DttRequestSearchForm()); }
            set { _search = value; }
        }

        public List<DttRequestOrderForm> order {
            get { return _order ?? (_order = new List<DttRequestOrderForm>()); }
            set { _order = value; }
        }

        public List<DttRequestColumnForm> columns {
            get { return _columns ?? (_columns = new List<DttRequestColumnForm>()); }
            set { _columns = value; }
        }

        public DttRequestColumnForm firstOrderColumn {
            get {
                var ord = order.FirstOrDefault();
                if (ord == null) return null;
                if (ord.column < 0 || ord.column >= columns.Count) return null;
                return columns[ord.column];
            }
        }

        public List<DttDynamicSearch> dyamicSearch { get; set; }

        public bool? status
        {
            get
            {
                return _status;
            }
            set { _status = value; }
        }
    }

    public class DttDynamicSearch {
        public string logicgate { get; set; }
        public string value { get; set; }
        public string filter { get; set; }
        public string data { get; set; }
        public int index { get; set; }
        public string type { get; set; }
    }

    public class DttRequestSearchForm {
        public string value { get; set; }
        public string regex { get; set; }
    }

    public class DttRequestOrderForm {
        private string _dir;

        public int column { get; set; }
        public string colname { get; set; }

        public string dir {
            get { return string.IsNullOrWhiteSpace(_dir) ? "asc" : _dir; }
            set { _dir = value; }
        }

        public bool IsDesc => dir == "desc";
        public bool IsAsc => dir != "desc";
    }

    public class DttRequestColumnForm {
        private DttRequestSearchForm _search;

        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }

        public DttRequestSearchForm search {
            get { return _search ?? (_search = new DttRequestSearchForm()); }
            set { _search = value; }
        }
    }
}