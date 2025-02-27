namespace PAUYS.DTO.Abstract
{
    public interface IBaseDto
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Deleted { get; set; }

        public bool IsDeleted { get; set; }

    }
}
