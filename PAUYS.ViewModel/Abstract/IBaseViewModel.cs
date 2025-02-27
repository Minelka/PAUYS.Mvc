namespace PAUYS.ViewModel.Abstract
{
    public interface IBaseViewModel<TKey>
    {
        TKey Id { get; set; }
        DateTime Created { get; set; }
        DateTime? Updated { get; set; }
        DateTime? Deleted { get; set; }

    }
}
