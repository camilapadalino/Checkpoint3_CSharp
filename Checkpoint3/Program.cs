using System;
using System.Collections.Generic;
using System.Globalization;

abstract class Operation
{
    private static int _nextId = 1;
    public int Id { get; }
    public DateTime Timestamp { get; }
    public string Ticker { get; }
    public int Quantity { get; }
    public decimal Price { get; }

    public Operation(string ticker, int quantity, decimal price)
    {
        Id = _nextId++;
        Timestamp = DateTime.Now;
        Ticker = ticker;
        Quantity = quantity;
        Price = price;
    }

    public decimal Total => Quantity * Price;

    public abstract string GetDetails();
    public abstract string Type { get; }
}

class BuyOperation : Operation
{
    public BuyOperation(string ticker, int quantity, decimal price)
        : base(ticker, quantity, price) { }

    public override string GetDetails()
    {
        return $"COMPRA: [{Id:D3}] {Timestamp:dd/MM/yyyy HH:mm} - {Ticker} x{Quantity} @ R$ {Price:F2} = R$ {Total:F2}";
    }

    public override string Type => "COMPRA";
}

class SellOperation : Operation
{
    public SellOperation(string ticker, int quantity, decimal price)
        : base(ticker, quantity, price) { }

    public override string GetDetails()
    {
        return $"VENDA:  [{Id:D3}] {Timestamp:dd/MM/yyyy HH:mm} - {Ticker} x{Quantity} @ R$ {Price:F2} = R$ {Total:F2}";
    }

    public override string Type => "VENDA";
}

class Program
{
    static List<Operation> operations = new List<Operation>();

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n--- MENU ---");
            Console.WriteLine("1 - Registrar operação");
            Console.WriteLine("2 - Listar operações");
            Console.WriteLine("3 - Mostrar valor total");
            Console.WriteLine("0 - Sair");
            Console.Write("Escolha uma opção: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    RegisterOperation();
                    break;
                case "2":
                    ListOperations();
                    break;
                case "3":
                    ShowTotals();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }
        }
    }

    static void RegisterOperation()
    {
        Console.Write("Tipo de operação (1=Compra, 2=Venda): ");
        string typeInput = Console.ReadLine();
        if (typeInput != "1" && typeInput != "2")
        {
            Console.WriteLine("Tipo inválido.");
            return;
        }

        Console.Write("Código do Ativo (ex: PETR4): ");
        string ticker = Console.ReadLine().ToUpper();

        Console.Write("Quantidade: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
        {
            Console.WriteLine("Quantidade inválida.");
            return;
        }

        Console.Write("Preço: ");
        if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price <= 0)
        {
            Console.WriteLine("Preço inválido.");
            return;
        }

        Operation op = typeInput == "1"
            ? new BuyOperation(ticker, quantity, price)
            : new SellOperation(ticker, quantity, price);

        operations.Add(op);
        Console.WriteLine("Operação registrada com sucesso.");
    }

    static void ListOperations()
    {
        if (operations.Count == 0)
        {
            Console.WriteLine("Nenhuma operação registrada.");
            return;
        }

        Console.WriteLine("\n--- Histórico de Operações ---");
        foreach (var op in operations)
        {
            Console.WriteLine(op.GetDetails());
        }
    }

    static void ShowTotals()
    {
        decimal totalCompras = 0;
        decimal totalVendas = 0;

        foreach (var op in operations)
        {
            if (op is BuyOperation)
                totalCompras += op.Total;
            else if (op is SellOperation)
                totalVendas += op.Total;
        }

        Console.WriteLine($"\nValor total de compras: R$ {totalCompras:F2}");
        Console.WriteLine($"Valor total de vendas:  R$ {totalVendas:F2}");
    }
}