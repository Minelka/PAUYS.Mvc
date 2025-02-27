namespace PAUYS.DTO.Abstract
{
    public abstract class BaseDto : IBaseDto
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? Deleted { get; set; }
        public bool Status { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
