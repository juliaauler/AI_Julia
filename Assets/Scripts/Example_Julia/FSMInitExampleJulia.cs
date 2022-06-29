using ChallengeAI;

public class FSMInitExampleJulia : FSMInitializer //IFSMInitializer
{
  public override string Name => "Robo Julia";
  public override void Init()
  {
    RegisterState<IdleState>(ExampleStateJulia.IDLE);
    RegisterState<CaptureFlag>(ExampleStateJulia.CAPTURE_FLAG);
    RegisterState<WalkToEnergy>(ExampleStateJulia.WALK_ENERGY);
    RegisterState<WalkToMunition>(ExampleStateJulia.WALK_MUNITION);
    RegisterState<CatchEminem>(ExampleStateJulia.CATCH_EMINEM);
    RegisterState<ToMyFlag>(ExampleStateJulia.TO_MY_FLAG);

  }
}
