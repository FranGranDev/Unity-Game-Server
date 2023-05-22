public interface IUiBehavior
{
    void Initilize(bool startState = false);
    bool IsShown { get; set; }
    bool IsPlaying { get; }
}
