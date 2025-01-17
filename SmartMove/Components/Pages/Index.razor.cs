namespace SmartMove.Components.Pages
{
    public partial class Index
    {
        /// This method is called when the component is initialized.
        /// It automatically navigates the user to the Dashboard page ("/login").
        protected override void OnInitialized()
        {
            Nav.NavigateTo("/login");
        }
    }
}
