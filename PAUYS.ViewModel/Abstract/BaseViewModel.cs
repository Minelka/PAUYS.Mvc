using System.ComponentModel.DataAnnotations;

namespace PAUYS.ViewModel.Abstract
{
    public abstract class BaseViewModel<TKey> : IBaseViewModel<TKey>
    {
        private static int _rowNumber;

        protected BaseViewModel(int rowNumber)
        {
            _rowNumber = rowNumber;
        }

        public virtual int RowNumber
        {
            get
            {
                _rowNumber = _rowNumber + 1;
                return _rowNumber;
            }

            set
            {
                _rowNumber = value;
            }
        }

        [Display(Name = "ID")]
        public TKey Id { get; set; }

        [Display(Name = "Durumu")]
        public bool Status { get; set; }

        [Display(Name = "Oluşturma Tarihi")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Display(Name = "Güncelleme Tarihi")]
        public DateTime? Updated { get; set; }

        public bool IsDeleted { get; set; }

        [Display(Name = "Silinme Tarihi")]
        public DateTime? Deleted { get; set; }
    }
}
