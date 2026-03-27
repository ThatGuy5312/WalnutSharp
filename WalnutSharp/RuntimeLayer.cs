using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalnutSharp;

public class RuntimeLayer
{
    // runs before game window starts
    public virtual void OnInit() { }

    // runs on loading the game window
    public virtual void OnStart() { }

    // runs every frame, run functions that need to be contantly updated
    public virtual void OnUpdate(double time) { }

    // runs every frame, run functions that need to be rendered every frame
    public virtual void OnRender(double time) { }
}
