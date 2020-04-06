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
      public override string ToString() { return "S: " + volt.ToString("N1"); }
    }

    public class Resistor : Component
    {
      public double ohm;
      public Resistor(double ohm, Node node1, Node node2) : base(node1, node2) { this.ohm = ohm; }
      public override string ToString() { return "R: " + ohm.ToString("N0"); }
    }

    public class Node
    {
      public List<Component> components = new List<Component>();
    }

    public static void Calc(Node ground)
    {
      // --- Netzwerk scannen ---
      var nodes = new HashSet<Node>();
      var components = new HashSet<Component>();
      Action<Node> addNode = null;
      addNode = n =>
      {
        if (n == null || nodes.Contains(n)) return;
        nodes.Add(n);
        foreach (var el in n.components)
        {
          components.Add(el);
          addNode(el.node1);
          addNode(el.node2);
        }
      };
      addNode(ground);


    }

    static void Main(string[] args)
    {
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

      var ground = new Node();
      var wire10V = new Node();
      var wire20V = new Node();
      var wireMiddle = new Node();

      var source10V = new VoltageSource(10, ground, wire10V);
      var source20V = new VoltageSource(20, ground, wire20V);
      var r10 = new Resistor(10, wire10V, wireMiddle);
      var r20 = new Resistor(20, wireMiddle, ground);
      var r30 = new Resistor(30, wireMiddle, wire20V);

      Calc(ground);
    }
  }
}
