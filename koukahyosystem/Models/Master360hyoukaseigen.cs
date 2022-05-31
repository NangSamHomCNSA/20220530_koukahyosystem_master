using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class Master360hyoukaseigen
    {
        public string cur_year { get; set; }
        public IEnumerable<SelectListItem> YearList { get; set; }

        [Required(ErrorMessage = "* 制限を入力してください。")]
        [Display(Name = "制限")]
        public string SeigenName { get; set; }

        public bool f_nentou { get; set; }

        public String txt_disabled { get; set; }
    }
}