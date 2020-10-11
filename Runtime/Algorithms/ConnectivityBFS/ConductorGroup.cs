using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConductorGroup
{
    protected ConnectivitySolver solver;

    protected bool dirtyBit = false;

    public bool IsOn
    {
        get
        {
            return solver.AndPropertyBitsDefaultGroup == 0;
        }
    }

    public ConductorGroup(List<ElectricConductor> conductors)
    {
        solver = new ConnectivitySolver(conductors.Select((x) => { return x as IConnectivityNode; }).ToList());
    }

    public ConductorGroup(ConnectivitySolver solver)
    {
        this.solver = solver;
    }

    //mark it for resolving. 
    public void SetDirty()
    {
        dirtyBit = true;
    }

    //resolve the group. Only execute if dirty is set
    //return if there is a new resolving
    public void ResolveIfDirty()
    {
        if (dirtyBit == false)
            return;

        solver.Resolve();
        var newSolvers = solver.Split();

        foreach (var groupSolver in newSolvers)
        {
            ConductorGroup group = null;
            if (groupSolver == solver)
            {
                //the same group as this
                group = this;
                group.solver = groupSolver;
            }
            else
            {
                group = new ConductorGroup(groupSolver);
            }
           
            var nodes = groupSolver.GetNodes(0);
            foreach(var node in nodes)
            {
                var newNode = (node as ElectricConductor);
                newNode.Group = group;
            }

            group.dirtyBit = false;
        }
        return ;
    }

}
