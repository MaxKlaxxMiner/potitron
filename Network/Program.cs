#region # using *.*
// ReSharper disable RedundantUsingDirective
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Solvers;
// ReSharper disable AccessToModifiedClosure

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
      public double potential;
      public List<Component> components = new List<Component>();
      public override string ToString()
      {
        return potential.ToString("N2") + " V";
      }
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
      var sources = componentsHash.Where(x => x is VoltageSource).Cast<VoltageSource>().ToArray();
      var passives = componentsHash.Where(x => x is Resistor).Cast<Resistor>().ToArray();

      int n = nodes.Length;
      int m = sources.Length;

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

      // --- Matrix B für die Spannungsquellen berechnen ---
      var matrixB = new int[n, m]; // y/n = Knoten, x/m = Spannungsquelle
      for (int y = 0; y < n; y++)
      {
        for (int x = 0; x < m; x++)
        {
          if (sources[x].node1 == nodes[y]) matrixB[y, x] = 1;
          if (sources[x].node2 == nodes[y]) matrixB[y, x] = -1;
        }
      }

      // --- Matrix C für die Spannungsquellen berechnen ---
      var matrixC = new int[m, n]; // y/m = Spannungsquelle, x/n = Knoten
      for (int y = 0; y < m; y++)
      {
        for (int x = 0; x < n; x++)
        {
          if (sources[y].node1 == nodes[x]) matrixC[y, x] = 1;
          if (sources[y].node2 == nodes[x]) matrixC[y, x] = -1;
        }
      }

      // --- Matrix D erstellen ---
      var matrixD = new int[m, m];

      // --- x-Matrix vorbereiten ---
      var vMatrixNodeVoltages = new double[n];   // Spannungen an den Knotenpunkten
      var jMatrixSourceCurrents = new double[m]; // Ströme an den Spannungsquellen

      // --- z-Matrix vorbereiten ---
      var iMatrixNodeCurrents = new double[n];   // Summe der Ströme angeschlossener passiver Komponenten
      var eMatrixSourceVoltages = new double[m]; // Spannungen der Spannungsquellen

      for (int x = 0; x < n; x++)
      {
        iMatrixNodeCurrents[x] = 0;
      }
      for (int x = 0; x < m; x++)
      {
        eMatrixSourceVoltages[x] = sources[x].volt;
      }

      // --- Matrix A zusammenstellen ---
      var matrixA = new double[n + m, n + m];
      for (int y = 0; y < n; y++)
      {
        for (int x = 0; x < n; x++)
        {
          matrixA[y, x] = matrixG[y, x];
        }
      }
      for (int y = 0; y < n; y++)
      {
        for (int x = 0; x < m; x++)
        {
          matrixA[y, x + n] = matrixB[y, x];
        }
      }
      for (int y = 0; y < m; y++)
      {
        for (int x = 0; x < n; x++)
        {
          matrixA[y + n, x] = matrixC[y, x]; // matrixC kann durch matrixB (nur transponiert)
        }
      }
      for (int y = 0; y < m; y++)
      {
        for (int x = 0; x < m; x++)
        {
          matrixA[y + n, x + n] = matrixD[y, x]; // theoretisch überflüssig (= 0)
        }
      }

      Matrix<double> matA = SparseMatrix.OfArray(matrixA);
      Vector<double> vec = SparseVector.OfEnumerable(iMatrixNodeCurrents.Concat(eMatrixSourceVoltages));

      var result = matA.Solve(vec);
      for (int i = 0; i < nodes.Length; i++) nodes[i].potential = result[i];

      // todo: https://lpsa.swarthmore.edu/Systems/Electrical/mna/MNA3.html#Putting_it_Together
    }

    #region # static readonly double[] Resistors =
    static readonly double[] Resistors =
    {
      300,
      330,
      360,
      390,
      430,
      470,
      510,
      560,
      620,
      680,
      750,
      820,
      910,
      1000,
      1100,
      1200,
      1300,
      1500,
      1600,
      1800,
      2000,
      2200,
      2400,
      2700,
      3000,
      3300,
      3600,
      3900,
      4300,
      4700,
      5100,
      5600,
      6200,
      6800,
      7500,
      8200,
      9100,
      10000,
      10000,
      11000,
      12000,
      13000,
      15000,
      16000,
      18000,
      20000,
      22000,
      24000,
      27000,
      30000,
      33000,
      36000,
      39000,
      43000,
      47000,
      51000,
      56000,
      62000,
      68000,
      75000,
      82000,
      91000,
      100000
    };
    #endregion

    static void Main(string[] args)
    {
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

      var rnd = new Random();

      var rValues = new double[] { 1000, 1000, 1074, 1807 };

      var ground = new Node();
      var node5V = new Node();
      var outputNode = new Node();
      var source5V = new VoltageSource(5, node5V, ground);

      var rPlus = new Resistor(rValues[0], node5V, outputNode);
      var rMinus = new Resistor(rValues[1], ground, outputNode);

      var pins = new KeyValuePair<VoltageSource, Resistor>[rValues.Length - 2];
      for (int i = 0; i < pins.Length; i++)
      {
        var n = new Node();
        var s = new VoltageSource(5, n, ground);
        var r = new Resistor(rValues[i + 2], n, outputNode);
        pins[i] = new KeyValuePair<VoltageSource, Resistor>(s, r);
      }

      int stateCount = 1;
      for (int i = 0; i < pins.Length; i++) stateCount *= 3;

      int selectValue = 0;
      double bestPoints = -1;
      string bestValues = "";
      for (; ; )
      {
        var outputValues = new List<KeyValuePair<string, double>>();

        for (int state = 0; state < stateCount; state++)
        {
          int stateValue = state;
          string stateStr = "";
          for (int pin = 0; pin < pins.Length; pin++)
          {
            int pinState = stateValue % 3;
            stateValue /= 3;
            stateStr += pinState;
            switch (pinState)
            {
              case 0: pins[pin].Key.volt = 0; pins[pin].Value.ohm = rValues[pin + 2]; break; // LOW
              case 1: pins[pin].Key.volt = 5; pins[pin].Value.ohm = 100000000; break;        // INPUT
              case 2: pins[pin].Key.volt = 5; pins[pin].Value.ohm = rValues[pin + 2]; break; // HIGH
              default: throw new Exception();
            }
          }

          Calc(ground);
          outputValues.Add(new KeyValuePair<string, double>(stateStr, outputNode.potential));
        }

        outputValues.Sort((x, y) => x.Value.CompareTo(y.Value));
        double min = outputValues.Min(x => x.Value);
        double max = outputValues.Max(x => x.Value);
        double maxGap = 0;
        for (int i = 1; i < outputValues.Count; i++)
        {
          double gap = outputValues[i].Value - outputValues[i - 1].Value;
          if (gap > maxGap) maxGap = gap;
        }

        double points = 0;
        double dynamic = max - min;
        if (dynamic > 1.5)
        {
          points = dynamic / maxGap / (stateCount - 1);
        }

        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("  Resistors: " + string.Join(" -", rValues.Skip(2).Select((x, i) => selectValue == i ? ">"+x.ToString("N0")  : " "+x.ToString("N0"))));
        Console.WriteLine();
        Console.WriteLine("  Precision: {0:N5} %", points);
        Console.WriteLine();
        foreach (var state in outputValues)
        {
          Console.WriteLine("{0}: {1:N5} V", string.Join(" ", state.Key).PadLeft(11), state.Value);
        }
        Console.WriteLine();

        if (points > bestPoints)
        {
          bestPoints = points;
          bestValues = string.Join(" - ", rValues.Skip(2).Select((x, i) => x.ToString("N0")));
        }
        Console.WriteLine();
        Console.WriteLine("       Best: " + bestValues);
        Console.WriteLine();
        Console.WriteLine("             " + bestPoints.ToString("N5") + " %");

        switch (Console.ReadKey().Key)
        {
          case ConsoleKey.Add:
          case ConsoleKey.OemPlus: rValues[selectValue + 2]++; break;
          case ConsoleKey.Subtract:
          case ConsoleKey.OemMinus: rValues[selectValue + 2]--; break;
          case ConsoleKey.Divide: selectValue--; if (selectValue < 0) selectValue = pins.Length - 1; break;
          case ConsoleKey.Multiply:
          case ConsoleKey.Spacebar: selectValue++; if (selectValue >= pins.Length) selectValue = 0; break;
          case ConsoleKey.Escape: break;
        }
      }
    }
  }
}
