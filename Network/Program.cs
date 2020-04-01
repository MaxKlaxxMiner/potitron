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
#pragma warning disable 649
#pragma warning disable 169
#endregion

namespace Network
{
  class Program
  {
    class Node
    {
      public double volt;
      public List<Element> connected = new List<Element>();
      public Node(double volt)
      {
        this.volt = volt;
      }
      public override string ToString()
      {
        return volt.ToString("N1") + " V";
      }
    }

    class FixNode : Node
    {
      public FixNode(double volt) : base(volt) { }
      public override string ToString()
      {
        return volt == 0 ? "Ground" : "Source " + base.ToString();
      }
    }

    class WireNode : Node
    {
      public WireNode() : base(0.0) { }
    }

    abstract class Element
    {
      public Node node1;
      public Node node2;
      public double ampere;
      public FixNode FixNode { get { return node1 as FixNode ?? node2 as FixNode; } }
    }

    class Resistor : Element
    {
      public double ohm;
      public Resistor(double ohm, Node node1, Node node2)
      {
        this.ohm = 1 / ohm;
        this.node1 = node1;
        this.node2 = node2;
        node1.connected.Add(this);
        node2.connected.Add(this);
      }
      public override string ToString()
      {
        return (1 / ohm).ToString("N0") + " ohm, " + (ampere * 1000.0).ToString("N1") + " mA, " + Math.Abs(node1.volt - node2.volt).ToString("N1") + " V";
      }
    }

    static void UpdateNetwork(Node anyNode)
    {
      // --- dazugehäörige Netzwerk-Elemente sammeln ---
      var nodes = new HashSet<Node>();
      var elements = new HashSet<Element>();
      Action<Node> addNode = null;
      addNode = n =>
      {
        if (n == null || nodes.Contains(n)) return;
        nodes.Add(n);
        foreach (var el in n.connected)
        {
          elements.Add(el);
          addNode(el.node1);
          addNode(el.node2);
        }
      };
      addNode(anyNode);

      // --- alle möglichen Maschen suchen ---
      var allMeshes = new List<Element[]>();
      foreach (var n in nodes.OrderBy(x => -x.volt))
      {
        var fix = n as FixNode;
        if (fix == null) continue;
        allMeshes.AddRange(Gen(fix, fix, new List<Element>()).Select(list => list.ToArray()));
      }

      // --- linear unabhängig Maschen ermitteln ---
      var usedElements = new HashSet<Element>();
      var meshes = new List<Element[]>();
      foreach (var newMesh in allMeshes.OrderBy(x => x.Last().FixNode.volt).ThenBy(x => x.First().FixNode.volt))
      {
        if (newMesh.All(el => usedElements.Contains(el))) continue;
        foreach (var el in newMesh) usedElements.Add(el);
        meshes.Add(newMesh);
      }


    }

    static IEnumerable<List<Element>> Gen(FixNode first, Node node, List<Element> list)
    {
      foreach (var el in node.connected)
      {
        if (list.Contains(el)) continue;

        var nextNode = el.node1 != node ? el.node1 : el.node2;

        list.Add(el);

        if (nextNode is FixNode)
        {
          if (nextNode.volt < first.volt) yield return list;
        }
        else
        {
          foreach (var masche in Gen(first, nextNode, list))
          {
            yield return masche;
          }
        }

        list.RemoveAt(list.Count - 1);
      }
    }

    static void Main(string[] args)
    {
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

      var ground = new FixNode(0);
      var u1 = new FixNode(10);
      var u2 = new FixNode(20);
      var w = new WireNode();
      var r1 = new Resistor(10, u1, w);
      var r2 = new Resistor(20, w, ground);
      var r3 = new Resistor(30, w, u2);

      UpdateNetwork(ground);
    }
  }
}
