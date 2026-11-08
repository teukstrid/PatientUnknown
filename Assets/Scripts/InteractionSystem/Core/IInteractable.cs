
public interface IInteractable
{
    public bool CanInteract(); 
    public bool Interact(Interactor interactor);
    void OnFocusGained() {}
    void OnFocusLost() {}
    public bool ShouldShowOutline() { return false; }
}