using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ChallengeAI;

public class ExampleStateJulia {
  static public string IDLE = "idle";
  static public string WALK = "walk";
  static public string CAPTURE_FLAG = "capture_flag";
  static public string WALK_ENERGY = "walk_energy";
  static public string WALK_MUNITION = "walk_munition";
  static public string CATCH_EMINEM = "catch_eminem";
  static public string TO_MY_FLAG = "to_my_flag";
  

  static public string[] StateNames = new string[] {
    IDLE,
    CAPTURE_FLAG,
    WALK_ENERGY,
    WALK_MUNITION,
    CATCH_EMINEM,
    TO_MY_FLAG
  };
}

public class IdleState : State {
  public IdleState(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
  private float angle;
    
  public override void Enter() {
    Agent.Stop();
    Log($"Idle");
  }

  public override void Exit() {
    Log();
  }

  public override void Update(float deltaTime) {
    if (Agent.Data.Energy >= 40f || Agent.Data.Energy <= 50F) {
      foreach (var powerUp in Agent.Data.PowerUps) {
        if (Vector3.Distance(Agent.Data.Position, powerUp) <= 15f) {
          var nextState = ExampleStateJulia.WALK_ENERGY;
          ChangeState(nextState);
          Log($"NextState:{nextState}");
        }
      }
    }  
    if (Agent.Data.Energy >= 51f) {
      if (Agent.Data.HasFlag) {
        var nextState = ExampleStateJulia.TO_MY_FLAG;
        ChangeState(nextState);
        Log($"NextState:{nextState}");
      }
      else {
        var nextState = ExampleStateJulia.CAPTURE_FLAG;
        ChangeState(nextState);
        Log($"NextState:{nextState}");
      }
    }
    if (Agent.Data.HasSightEnemy && Agent.Data.Ammo > 0 && !Agent.Data.IsCooldownFire) {
      Agent.Fire();
    }
    angle++;
    Agent.Rotate(angle);
  }
}

public class WalkState : State {
  public WalkState(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
  public Vector3 Destination { get; set; } = Vector3.zero;
  public override void Enter() {
    Agent.Move(Destination);
    Log($"Destination:{Destination}");
  }
  public override void Exit() {
    Log();
  }
  public override void Update(float deltaTime) { }
}

public class CaptureFlag : WalkState {
  public CaptureFlag(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
  public override void Enter() {
    Destination = (Vector3)Agent.EnemyData[0].FlagPosition;
    // Destination = (Vector3)Agent.Data.AmmoRefill.ElementAtOrDefault(0);
    // Destination = (Vector3)Agent.Data.PowerUps.ElementAtOrDefault(0);
    Log($"Flag Destination {Destination}");
    base.Enter();
  }
  public override void Update(float deltaTime) {
    //caminha apenas se estiver com mais de 50% de energia
    if(Agent.Data.Energy <= 3) {
      var nextState = ExampleStateJulia.IDLE;
      ChangeState(nextState);
      Log($"NextState:{nextState}");
    }
    //se energia entre 20 e 30%, buscar energia mais proxima
    //Agent.Data.Energy >= 20 || Agent.Data.Energy <= 30
    //Agent.Data.Energy <= 40
    else if (Agent.Data.Energy >= 20 || Agent.Data.Energy <= 30) {
      foreach (var powerUp in Agent.Data.PowerUps) {
        if (Vector3.Distance(Agent.Data.Position, powerUp) <= 15f) {
          var nextState = ExampleStateJulia.WALK_ENERGY;
          ChangeState(nextState);
          Log($"NextState:{nextState}");
        }
      }
    }

    if (Agent.Data.HasSightEnemy && Agent.Data.Ammo > 0 && !Agent.Data.IsCooldownFire) {
      Agent.Fire();
      Agent.Move(Destination);
    }
    //se está com 1 de municao, buscar municao mais proxima
    else if (Agent.Data.Ammo == 0) {
      var nextState = ExampleStateJulia.WALK_MUNITION;
      ChangeState(nextState);
      Log($"NextState:{nextState}");
    }

    if (Agent.Data.HasFlag) {
      var nextState = ExampleStateJulia.TO_MY_FLAG;
      ChangeState(nextState);
      Log($"NextState:{nextState}");
    }

    //caminha apenas se estiver com mais de 50% de energia
    //if(Agent.Data.Energy >= 50) {
    //  var nextState = ExampleStateJulia.IDLE;
    //  ChangeState(nextState);
    //  Log($"NextState:{nextState}");
    //}

    //se energia entre 20 e 30%, buscar energia mais proxima
    //if(Agent.Data.Energy >= 20 || Agent.Data.Energy <= 30) {
    //  var nextState = ExampleStateJulia.WALK_ENERGY;
    //  ChangeState(nextState);
    //  Log($"NextState:{nextState}");
    //}

    //se está com 1 de municao, buscar municao mais proxima
    //if(Agent.Data.Energy <= 1) {
    //  var nextState = ExampleStateJulia.WALK_MUNITION;
    // ChangeState(nextState);
    //  Log($"NextState:{nextState}");
    //}
  }
}

public class ToMyFlag : WalkState {
  public ToMyFlag(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
  public override void Enter() {
    Destination = (Vector3) Agent.Data.FlagPosition;
    Log($"My Flag Destination {Destination}");
    base.Enter();
  }
    
  public override void Update(float deltaTime) {
    if (Agent.Data.Energy <= 3) {
      var nextState = ExampleStateJulia.IDLE;
      ChangeState(nextState);
      Log($"NextState:{nextState}");
    }
    else if (Agent.Data.Energy >= 20 || Agent.Data.Energy <= 30) {
      foreach (var powerUp in Agent.Data.PowerUps) {
        if (Vector3.Distance(Agent.Data.Position, powerUp) <= 15f) {
          var nextState = ExampleStateJulia.WALK_ENERGY;
          ChangeState(nextState);
          Log($"NextState:{nextState}");
        }
      }
    }
    if (Agent.Data.HasSightEnemy && Agent.Data.Ammo > 0 && !Agent.Data.IsCooldownFire) {
      Agent.Fire();
      Agent.Move(Destination);
    } 
    else if (Agent.Data.Ammo == 0 && Agent.Data.FlagState == FlagState.Catched) {
      var nextState = ExampleStateJulia.WALK_MUNITION;
      ChangeState(nextState);
      Log($"NextState:{nextState}");
    }

    if (Agent.Data.FlagState == FlagState.Catched) {
      var nextState = ExampleStateJulia.CATCH_EMINEM;
      ChangeState(nextState);
      Log($"NextState:{nextState}");
    }
        
    if (!Agent.Data.HasFlag) {
      var nextState = ExampleStateJulia.CAPTURE_FLAG;
      ChangeState(nextState);
      Log($"NextState:{nextState}");
    }
  }
}

public class WalkToEnergy : WalkState {
  public WalkToEnergy(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
  private float currentDistance;
  private List<float> energyDistances;
  private int closestEnergy;

  public override void Enter() {
    if (Agent.Data.Position.x <= 0) {
      Destination = (Vector3) Agent.Data.PowerUps[1];    
    }
    else {
      Destination = (Vector3) Agent.Data.PowerUps[0];
    }
    Log($"Energy Destination {Destination}");
    base.Enter();
  }

  public override void Update(float deltaTime) {
    if (Agent.Data.RemainingDistance <= 0.05f) {
      if (Agent.Data.HasFlag) {
        var nextState = ExampleStateJulia.TO_MY_FLAG;
        ChangeState(nextState);
        Log($"NextState:{nextState}");
      }
      else {
        var nextState = ExampleStateJulia.CAPTURE_FLAG;
        ChangeState(nextState);
        Log($"NextState:{nextState}");
      }
    }
    if (Agent.Data.HasSightEnemy && Agent.Data.Ammo > 0 && !Agent.Data.IsCooldownFire) {
      Agent.Fire();
      Agent.Move(Destination);
    }
  }
}

public class WalkToMunition : WalkState {
  public WalkToMunition(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
  
  public override void Enter() {
    if (Agent.Data.AmmoRefill == null) {
      var nextState = ExampleStateJulia.IDLE;
      ChangeState(nextState);
    }
    Destination = (Vector3) Agent.Data.AmmoRefill[0];
    Log($"Munition destination: {Destination}");
    base.Enter();
  }
    
  public override void Update(float deltaTime) {
    if (Agent.Data.Ammo >= 2) {
      if (Agent.Data.HasFlag) {
        var nextState = ExampleStateJulia.TO_MY_FLAG;
        ChangeState(nextState);
        Log($"NextState:{nextState}");
      }
      else {
        var nextState = ExampleStateJulia.CAPTURE_FLAG;
          ChangeState(nextState);
          Log($"NextState:{nextState}");
      }
    }
        
    if (Agent.Data.Energy <= 3) {
      var nextState = ExampleStateJulia.IDLE;
      ChangeState(nextState);
      Log($"NextState:{nextState}");
    }
    else if (Agent.Data.Energy >= 20 || Agent.Data.Energy <= 30) {
      foreach (var powerUp in Agent.Data.PowerUps) {
        if (Vector3.Distance(Agent.Data.Position, powerUp) <= 15f) {
          var nextState = ExampleStateJulia.WALK_ENERGY;
          ChangeState(nextState);
          Log($"NextState:{nextState}");
        }
      }
    }
  }
}

public class CatchEminem : WalkState {
  public CatchEminem(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }

  public override void Enter() {
    Destination = Agent.Data.FlagPosition.Value;
    Log($"Enemy position: {Destination}");
    base.Enter();
  }

  public override void Update(float deltaTime) {
    if (Agent.Data.Energy <= 3) {
      var nextState = ExampleStateJulia.IDLE;
      Log($"NextState:{nextState}");
    }
    else if (Agent.Data.Energy >= 20 || Agent.Data.Energy <= 30) {
      foreach (var powerUp in Agent.Data.PowerUps) {
        if (Vector3.Distance(Agent.Data.Position, powerUp) <= 15f) {
          var nextState = ExampleStateJulia.WALK_ENERGY;
          ChangeState(nextState);
          Log($"NextState:{nextState}");
        }
      }
    }
        
    if (!Agent.Data.HasFlag) {
      var nextState = ExampleStateJulia.CAPTURE_FLAG;
      ChangeState(nextState);
      Log($"NextState:{nextState}");
    } 
    else if (Agent.Data.FlagState == FlagState.StartPosition) {
      var nextState = ExampleStateJulia.TO_MY_FLAG;
      ChangeState(nextState);
      Log($"NextState:{nextState}");
    }
        
    if (Agent.Data.HasSightEnemy && Agent.Data.Ammo > 0 && !Agent.Data.IsCooldownFire) {
      Agent.Fire();
      Agent.Move(Destination);
    } 
    else if (Agent.Data.Ammo == 0) {
      var nextState = ExampleStateJulia.WALK_MUNITION;
      ChangeState(nextState);
      Log($"NextState:{nextState}");
    }
    Destination = Agent.Data.FlagPosition.Value;
    Agent.Move(Destination);
  }
}