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
      public double cond;
      public double Ohm
      {
        get
        {
          return 1 / cond;
        }
        set
        {
          cond = 1 / value;
        }
      }
      public Resistor(double ohm, Node node1, Node node2) : base(node1, node2) { Ohm = ohm; }
      public override string ToString() { return "R: " + Ohm.ToString("N0") + " ohm"; }
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

    public class TristateScanner
    {
      public readonly double[] rValues;
      public readonly int stateCount;
      public readonly KeyValuePair<VoltageSource, Resistor>[] pins;
      readonly Node ground;
      readonly Node outputNode;

      public TristateScanner(double resHigh, double resLow, params double[] resPins)
      {
        if (resPins == null || resPins.Length == 0) throw new ArgumentNullException("resPins");
        rValues = new[] { resHigh, resLow }.Concat(resPins).ToArray();

        ground = new Node();
        outputNode = new Node();
        var node5V = new Node();
        var source5V = new VoltageSource(5, node5V, ground);

        var rPlus = new Resistor(rValues[0], node5V, outputNode);
        var rMinus = new Resistor(rValues[1], ground, outputNode);

        pins = new KeyValuePair<VoltageSource, Resistor>[rValues.Length - 2];
        for (int i = 0; i < pins.Length; i++)
        {
          var n = new Node();
          var s = new VoltageSource(5, n, ground);
          var r = new Resistor(rValues[i + 2], n, outputNode);
          pins[i] = new KeyValuePair<VoltageSource, Resistor>(s, r);
        }

        stateCount = 1;
        for (int i = 0; i < pins.Length; i++) stateCount *= 3;

        InitFastNetwork(ground);
      }

      public void CalcValues(KeyValuePair<string, double>[] outputValues)
      {
        //var mess = Stopwatch.StartNew();
        for (int state = 0; state < outputValues.Length; state++)
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
              case 0: pins[pin].Key.volt = 0; pins[pin].Value.Ohm = rValues[pin + 2]; break; // LOW
              case 1: pins[pin].Key.volt = 5; pins[pin].Value.Ohm = 100000000; break;        // INPUT (PULLUP)
              case 2: pins[pin].Key.volt = 5; pins[pin].Value.Ohm = rValues[pin + 2]; break; // HIGH
              default: throw new Exception();
            }
          }

          //Calc(ground);
          CalcFastNetwork();
          outputValues[state] = new KeyValuePair<string, double>(stateStr, outputNode.potential);
        }
        Array.Sort(outputValues, (x, y) => x.Value.CompareTo(y.Value));
        //mess.Stop();
        //Console.Title = (mess.ElapsedTicks * 1000.0 / Stopwatch.Frequency).ToString("N2") + " ms";
      }

      public static double GetPoints(KeyValuePair<string, double>[] outputValues)
      {
        double min = outputValues.Min(x => x.Value);
        double max = outputValues.Max(x => x.Value);
        double maxGap = 0;
        for (int i = 1; i < outputValues.Length; i++)
        {
          double gap = outputValues[i].Value - outputValues[i - 1].Value;
          if (gap > maxGap) maxGap = gap;
        }

        double dynamic = max - min;
        double points = dynamic / maxGap / (outputValues.Length - 1);
        return dynamic < 1.0 ? points * dynamic : points;
        //return dynamic < 3.0 ? points * dynamic * 0.33333333333333 : points;
      }

      interface IMatValue
      {
        double GetValue();
      }
      class MatValueFix : IMatValue
      {
        readonly double val;
        public MatValueFix(double val) { this.val = val; }
        public double GetValue() { return val; }
        public override string ToString() { return GetValue().ToString("N5"); }
      }
      class MatSingleResistor : IMatValue
      {
        public readonly Resistor resistor;
        public MatSingleResistor(Resistor resistor)
        {
          if (resistor == null) throw new ArgumentNullException("resistor");
          this.resistor = resistor;
        }
        public double GetValue() { return resistor.cond; }
        public override string ToString() { return GetValue().ToString("N5"); }
      }
      class MatResistors : IMatValue
      {
        public readonly Resistor[] resistors;
        public MatResistors(Resistor[] resistors)
        {
          if (resistors == null) throw new ArgumentNullException("resistors");
          this.resistors = resistors;
        }
        public double GetValue()
        {
          double sum = 0;
          foreach (var r in resistors) sum += r.cond;
          return sum;
        }
        public override string ToString() { return GetValue().ToString("N5"); }
      }
      class MatSingleResistorNeg : IMatValue
      {
        public readonly Resistor resistor;
        public MatSingleResistorNeg(Resistor resistor)
        {
          if (resistor == null) throw new ArgumentNullException("resistor");
          this.resistor = resistor;
        }
        public double GetValue() { return -resistor.cond; }
        public override string ToString() { return GetValue().ToString("N5"); }
      }
      class MatResistorsNeg : IMatValue
      {
        public readonly Resistor[] resistors;
        public MatResistorsNeg(Resistor[] resistors)
        {
          if (resistors == null) throw new ArgumentNullException("resistors");
          this.resistors = resistors;
        }
        public double GetValue()
        {
          double sum = 0;
          foreach (var r in resistors) sum += r.cond;
          return -sum;
        }
        public override string ToString() { return GetValue().ToString("N5"); }
      }
      class MatValueSource : IMatValue
      {
        public readonly VoltageSource source;
        public MatValueSource(VoltageSource source)
        {
          if (source == null) throw new ArgumentNullException("source");
          this.source = source;
        }
        public double GetValue() { return source.volt; }
        public override string ToString() { return GetValue().ToString("N5"); }
      }

      IMatValue[,] fastMatrix;
      IMatValue[] endVector;
      int outputNodeIndex;

      void InitFastNetwork(Node ground)
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

        var fix0 = new MatValueFix(0);
        var fix1 = new MatValueFix(1);
        var fixn1 = new MatValueFix(-1);

        // --- Matrix mit den Gleitwerten berechnen ---
        var matrixG = new IMatValue[n, n];
        for (int y = 0; y < n; y++)
        {
          for (int x = 0; x < n; x++)
          {
            matrixG[y, x] = fix0;

            if (x == y) // Hauptdiagonale = Summe der Leitwerte aller Zweige, die mit Knoten x verbunden sind
            {
              foreach (var c in nodes[x].components)
              {
                var r = c as Resistor;
                if (r == null) continue;
                if (matrixG[y, x] == fix0)
                {
                  matrixG[y, x] = new MatSingleResistor(r);
                }
                else
                {
                  var single = matrixG[y, x] as MatSingleResistor;
                  if (single != null)
                  {
                    matrixG[y, x] = new MatResistors(new[] { single.resistor, r });
                  }
                  else
                  {
                    matrixG[y, x] = new MatResistors(((MatResistors)matrixG[y, x]).resistors.Concat(new[] { r }).ToArray());
                  }
                }
              }
            }
            else // negative Summe der Leitwerte zwischen den benachbarten Knoten i und j (Koppelleitwerte). Besteht keine direkte Verbindung zwischen zwei Knoten, wird an dieser Stelle eine Null eingetragen.
            {
              var rs = nodes[x].components.Where(c => c is Resistor && nodes[y].components.Contains(c)).Cast<Resistor>().ToArray();
              if (rs.Length > 0)
              {
                if (rs.Length == 1)
                {
                  matrixG[y, x] = new MatSingleResistorNeg(rs[0]);
                }
                else
                {
                  matrixG[y, x] = new MatResistorsNeg(rs);
                }
              }
            }
          }
        }

        // --- Matrix B für die Spannungsquellen berechnen ---
        var matrixB = new IMatValue[n, m]; // y/n = Knoten, x/m = Spannungsquelle
        for (int y = 0; y < n; y++)
        {
          for (int x = 0; x < m; x++)
          {
            matrixB[y, x] = fix0;
            if (sources[x].node1 == nodes[y]) matrixB[y, x] = fix1;
            if (sources[x].node2 == nodes[y]) matrixB[y, x] = fixn1;
          }
        }

        // --- Matrix C für die Spannungsquellen berechnen ---
        var matrixC = new IMatValue[m, n]; // y/m = Spannungsquelle, x/n = Knoten
        for (int y = 0; y < m; y++)
        {
          for (int x = 0; x < n; x++)
          {
            matrixC[y, x] = fix0;
            if (sources[y].node1 == nodes[x]) matrixC[y, x] = fix1;
            if (sources[y].node2 == nodes[x]) matrixC[y, x] = fixn1;
          }
        }

        // --- Matrix D erstellen ---
        var matrixD = new IMatValue[m, m];
        for (int y = 0; y < m; y++)
        {
          for (int x = 0; x < m; x++)
          {
            matrixD[y, x] = fix0;
          }
        }

        endVector = new IMatValue[n + m];
        for (int x = 0; x < n; x++)
        {
          endVector[x] = fix0;
        }
        for (int x = 0; x < m; x++)
        {
          endVector[x + n] = new MatValueSource(sources[x]);
        }

        // --- Matrix A zusammenstellen ---
        var matrixA = new IMatValue[n + m, n + m];
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

        fastMatrix = matrixA;

        for (int i = 0; i < endVector.Length; i++) if (endVector[i] == fix0) endVector[i] = null;
        for (int y = 0; y < endVector.Length; y++)
        {
          for (int x = 0; x < endVector.Length; x++)
          {
            if (fastMatrix[y, x] == fix0) fastMatrix[y, x] = null;
          }
        }
      }

      void CalcFastNetwork()
      {
        int size = endVector.Length;
        var matArr = new double[size, size];
        for (int y = 0; y < size; y++)
        {
          for (int x = 0; x < size; x++)
          {
            var v = fastMatrix[y, x];
            if (v == null) continue;
            matArr[y, x] = v.GetValue();
          }
        }
        var vecArr = new double[size];
        for (int i = 0; i < vecArr.Length; i++)
        {
          var v = endVector[i];
          if (v == null) continue;
          vecArr[i] = v.GetValue();
        }

        Matrix<double> matA = DenseMatrix.OfArray(matArr);
        Vector<double> vec = DenseVector.OfArray(vecArr);

        var result = matA.Solve(vec);
        outputNode.potential = result[1];
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
                matrixG[y, x] += 1 / r.Ohm;
              }
            }
            else // negative Summe der Leitwerte zwischen den benachbarten Knoten i und j (Koppelleitwerte). Besteht keine direkte Verbindung zwischen zwei Knoten, wird an dieser Stelle eine Null eingetragen.
            {
              var rs = nodes[x].components.Where(c => c is Resistor && nodes[y].components.Contains(c)).Cast<Resistor>().ToArray();
              matrixG[y, x] = -rs.Sum(r => 1 / r.Ohm);
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

        Matrix<double> matA = DenseMatrix.OfArray(matrixA);
        Vector<double> vec = DenseVector.OfEnumerable(iMatrixNodeCurrents.Concat(eMatrixSourceVoltages));

        var result = matA.Solve(vec);
        for (int i = 0; i < nodes.Length; i++) nodes[i].potential = result[i];

        // todo: https://lpsa.swarthmore.edu/Systems/Electrical/mna/MNA3.html#Putting_it_Together
      }
    }

    static void Main(string[] args)
    {
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

      var scanner = new TristateScanner(2000, 2000, 1000, 1000, 1000, 1000);
      var rnd = new Random();

      var carlos = new List<KeyValuePair<double, double[]>>();
      double carloDif = 1;

      // --- 1 V ---
      // 2 Pins: 1073.99 - 1806.77
      // 3 Pins: 2,804.74 - 9,418.55 - 26,818.99
      // 4 Pins: 2,881.52 - 9,690.31 - 27,392.16 - 75,428.07
      // --- 3 V ---
      // 2 Pins: 516.94 - 938.50
      // 3 Pins: 389.01 - 746.99 - 1,302.12
      // 4 Pins: 624.16 - 1,211.94 - 2,135.60 - 5,603.45


      int selectValue = 0;
      var outputValues = new KeyValuePair<string, double>[scanner.stateCount];
      double bestPoints = -1;
      string bestValues = "";
      for (; ; )
      {
        scanner.CalcValues(outputValues);
        double points = TristateScanner.GetPoints(outputValues);
        if (points > bestPoints)
        {
          bestPoints = points;
          bestValues = string.Join(" - ", scanner.rValues.Skip(2).Select((x, i) => x.ToString("N2")));
        }

        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("  Resistors: " + string.Join(" -", scanner.rValues.Skip(2).Select((x, i) => selectValue == i ? ">" + x.ToString("N0") : " " + x.ToString("N0"))));
        Console.WriteLine();
        Console.WriteLine("  Precision: {0:N5} %", points * 100);
        Console.WriteLine();
        //foreach (var state in outputValues)
        //{
        //  Console.WriteLine("{0}: {1:N5} V", string.Join(" ", state.Key).PadLeft(11), state.Value);
        //}
        Console.WriteLine();

        Console.WriteLine();
        Console.WriteLine("       Best: " + bestValues);
        Console.WriteLine();
        Console.WriteLine("             " + (bestPoints * 100).ToString("N5") + " %");
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("     Carlos: " + carlos.Count.ToString("N0") + " / " + ((int)((Math.Log(carloDif) + 10) * 200)).ToString("N0") + " - (dif: " + carloDif.ToString("N8") + ")");
        Console.WriteLine();
        if (carlos.Count > 0) Console.WriteLine("             " + (carlos.Last().Key * 100).ToString("N5") + " %");

        int loop = 1;
        switch (Console.ReadKey().Key)
        {
          case ConsoleKey.Add:
          case ConsoleKey.OemPlus: scanner.rValues[selectValue + 2]++; break;
          case ConsoleKey.Subtract:
          case ConsoleKey.OemMinus: scanner.rValues[selectValue + 2]--; break;
          case ConsoleKey.Divide: selectValue--; if (selectValue < 0) selectValue = scanner.pins.Length - 1; break;
          case ConsoleKey.Multiply:
          case ConsoleKey.Spacebar: selectValue++; if (selectValue >= scanner.pins.Length) selectValue = 0; break;
          case ConsoleKey.D1: loop <<= 1; goto case ConsoleKey.R;
          case ConsoleKey.D2: loop <<= 2; goto case ConsoleKey.R;
          case ConsoleKey.D3: loop <<= 3; goto case ConsoleKey.R;
          case ConsoleKey.D4: loop <<= 4; goto case ConsoleKey.R;
          case ConsoleKey.D5: loop <<= 5; goto case ConsoleKey.R;
          case ConsoleKey.D6: loop <<= 6; goto case ConsoleKey.R;
          case ConsoleKey.D7: loop <<= 7; goto case ConsoleKey.R;
          case ConsoleKey.D8: loop <<= 8; goto case ConsoleKey.R;
          case ConsoleKey.D9: loop <<= 9; goto case ConsoleKey.R;
          case ConsoleKey.D0: loop <<= 10; goto case ConsoleKey.R;
          case ConsoleKey.R:
          {
            while (loop-- > 0)
            {
              var newSet = new double[scanner.pins.Length];

              if (carloDif < 1 || carlos.Count >= 1000)
              {
                int carloSelect = rnd.Next(0, rnd.Next(1, carlos.Count + 1));
                //int carloSelect = rnd.Next(0, carlos.Count);
                //int carloSelect = rnd.Next(rnd.Next(0, carlos.Count), carlos.Count);
                var oldSet = carlos[carloSelect].Value;
                for (int i = 0; i < newSet.Length; i++)
                {
                  newSet[i] = Math.Max(100, oldSet[i] + rnd.NextDouble() * carloDif * oldSet[i] - 0.5 * carloDif * oldSet[i]);
                }
                Array.Sort(newSet);
                for (int i = 0; i < newSet.Length; i++)
                {
                  scanner.rValues[i + 2] = newSet[i];
                }

                scanner.CalcValues(outputValues);
                points = TristateScanner.GetPoints(outputValues);

                if (points > carlos.Last().Key)
                {
                  int carloPos = carlos.Count;
                  while (carloPos > 0 && points > carlos[carloPos - 1].Key) carloPos--;
                  carlos.Insert(carloPos, new KeyValuePair<double, double[]>(points, newSet));
                }
                else
                {
                  carloDif *= 0.999;
                }
                while (carlos.Count > 10 && carlos.Count > (int)((Math.Log(carloDif) + 10) * 200)) carlos.RemoveAt(carlos.Count - 1);
              }
              else
              {
                for (int i = 0; i < newSet.Length; i++)
                {
                  newSet[i] = rnd.NextDouble() * 100000.0 + 100;
                }
                Array.Sort(newSet);
                for (int i = 0; i < newSet.Length; i++)
                {
                  scanner.rValues[i + 2] = newSet[i];
                }

                scanner.CalcValues(outputValues);
                points = TristateScanner.GetPoints(outputValues);
                int carloPos = carlos.Count;
                while (carloPos > 0 && points > carlos[carloPos - 1].Key) carloPos--;
                carlos.Insert(carloPos, new KeyValuePair<double, double[]>(points, newSet));
              }
            }
          } break;
          case ConsoleKey.Escape: return;
        }
      }
    }
  }
}
