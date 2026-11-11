
public interface IInteractable
{
    string PromptMessage { get; }
    public bool CanInteract(); 
    public bool Interact(Interactor interactor);
    void OnFocusGained() {}
    void OnFocusLost() {}
    public bool ShouldShowOutline() { return true; }
}