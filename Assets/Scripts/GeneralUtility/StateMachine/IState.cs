public interface IState 
{
    // update state
    void Tick();
    // on enter state
    void OnEnter();
    // on exit state
    void OnExit();
}
