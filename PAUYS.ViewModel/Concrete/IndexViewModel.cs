using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAUYS.ViewModel.Concrete
{
    public class IndexViewModel
    {
        public List<ProductViewModel> Products { get; set; }  // Ürünler listesi
        public List<CategoryViewModel> Categories { get; set; }  // Kategoriler listesi
        public List<CategoryViewModel> Materials { get; set; }  // Materyaller listesi
    }
}
