#region # using *.*
// ReSharper disable RedundantUsingDirective
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable PublicConstructorInAbstractClass
// ReSharper disable ClassCanBeSealed.Local
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBeProtected.Local
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable CollectionNeverQueried.Local
// ReSharper disable UnusedParameter.Global
// ReSharper disable VirtualMemberNeverOverriden.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable ClassCanBeSealed.Global
// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable UnusedVariable
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable RedundantIfElseBlock
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 649
#pragma warning disable 169
#endregion

namespace Network
{
  class Program
  {
    public abstract class Component
    {
      public Node node1;
      public Node node2;
      protected Component(Node node1, Node node2)
      {
        this.node1 = node1;
        this.node2 = node2;
        node1.components.Add(this);
        node2.components.Add(this);
      }
    }

    public class VoltageSource : Component
    {
      public double volt;
      public VoltageSource(double volt, Node node1, Node node2) : base(node1, node2) { this.volt = volt; }
      public override string ToString() { return "S: " + volt.ToString("N1") + " V"; }
    }

    public class Resistor : Component
    {
      public double ohm;
      public Resistor(double ohm, Node node1, Node node2) : base(node1, node2) { this.ohm = ohm; }
      public override string ToString() { return "R: " + ohm.ToString("N0") + " ohm"; }
    }

    public class Node
    {
      public List<Component> components = new List<Component>();
    }

    public static void Calc(Node ground)
    {
      // --- Netzwerk scannen ---
      var nodesHash = new HashSet<Node>();
      var componentsHash = new HashSet<Component>();
      Action<Node> addNode = null;
      addNode = x =>
      {
        if (x == null || nodesHash.Contains(x)) return;
        nodesHash.Add(x);
        foreach (var el in x.components)
        {
          componentsHash.Add(el);
          addNode(el.node1);
          addNode(el.node2);
        }
      };
      addNode(ground);

      var nodes = nodesHash.Where(x => x != ground).ToArray();
      var voltageSources = componentsHash.Where(x => x is VoltageSource).ToArray();

      int n = nodes.Length;
      int m = voltageSources.Length;

      //  matrixA: (m + n) * (m + n) - main matrix
      //    matrixG: n * n - interconnections between the passive circuit elements
      //    matrixB: n * m - connection of the voltage sources
      //    matrixC: m * n - connection of the voltage sources
      //    matrixD: m * m - zero if only independent sources are considered

      // --- Matrix mit den Gleitwerten berechnen ---
      var matrixG = new double[n, n];
      for (int y = 0; y < n; y++)
      {
        for (int x = 0; x < n; x++)
        {
          if (x == y) // Hauptdiagonale = Summe der Leitwerte aller Zweige, die mit Knoten x verbunden sind
          {
            foreach (var c in nodes[x].components)
            {
              var r = c as Resistor;
              if (r == null) continue;
              matrixG[y, x] += 1 / r.ohm;
            }
          }
          else // negative Summe der Leitwerte zwischen den benachbarten Knoten i und j (Koppelleitwerte). Besteht keine direkte Verbindung zwischen zwei Knoten, wird an dieser Stelle eine Null eingetragen.
          {
            var rs = nodes[x].components.Where(c => c is Resistor && nodes[y].components.Contains(c)).Cast<Resistor>().ToArray();
            matrixG[y, x] = -rs.Sum(r => 1 / r.ohm);
          }
        }
      }

      // --- Matrix für die Spannungsquellen berechnen ---
      var matrixB = new int[n, m]; // x/m = Spannungsquelle, y/n = Knoten
      for (int y = 0; y < n; y++)
      {
        for (int x = 0; x < m; x++)
        {
          if (voltageSources[x].node1 == nodes[y]) matrixB[y, x] = 1;
          if (voltageSources[x].node2 == nodes[y]) matrixB[y, x] = -1;
        }
      }
    }

    static void Main(string[] args)
    {
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

      //var ground = new Node();
      //var wire10V = new Node();
      //var wire20V = new Node();
      //var wireMiddle = new Node();

      //var source10V = new VoltageSource(10, ground, wire10V);
      //var source20V = new VoltageSource(20, ground, wire20V);
      //var r10 = new Resistor(10, wire10V, wireMiddle);
      //var r20 = new Resistor(20, wireMiddle, ground);
      //var r30 = new Resistor(30, wireMiddle, wire20V);

      // --- case 1 ---
      //var ground = new Node();
      //var node1 = new Node();
      //var node2 = new Node();
      //var node3 = new Node();
      //var r1 = new Resistor(2, node1, ground);
      //var r2 = new Resistor(4, node2, node3);
      //var r3 = new Resistor(8, node2, ground);
      //var source32V = new VoltageSource(32, node2, node1);
      //var source20V = new VoltageSource(20, node3, ground);

      // --- case 2 ---
      var ground = new Node();
      var node1 = new Node();
      var node2 = new Node();
      var r1 = new Resistor(2, node1, ground);
      var r2 = new Resistor(4, node1, node2);
      var r3 = new Resistor(8, node2, ground);
      var source32V = new VoltageSource(32, node1, node2);

      Calc(ground);
    }
  }
}
