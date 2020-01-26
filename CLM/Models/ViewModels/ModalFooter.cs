namespace CLM.Models
{
    public class ModalFooter
    {
        public string SubmitButtonText { get; set; } = "Guardar";
        public string CancelButtonText { get; set; } = "Cancelar";
        public string SubmitButtonID { get; set; } = "btn-submit";
        public string CancelButtonID { get; set; } = "btn-cancel";
        public bool OnlyCancelButton { get; set; }
    }
}