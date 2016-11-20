open System;

//Discriminated union
type Node<'a> =
    |Leaf 
    |Node of Node<string> * string * Node<string>

let generateBoilerPlate =
    @"using System;

namespace Transpiled
{
    class Program
    {
        static void Main(string[] args)
        {
            {0}
        }
    }
}
"

let extractStringParams (node:Node<string>,node2:Node<string>) = 
    match node,node2 with
    | Node(Leaf,a,Leaf),Node(Leaf,b,Leaf) -> a + " " + b
    | Leaf, Leaf -> ""
    | Leaf, Node(Leaf,a,Leaf) -> a
    | Node(Leaf,a,Leaf), Leaf -> a
    | _ -> ""


let transpileNode (node:Node<string>) =
    match node with
    | Node (a,b,c) when b = "Display" ->  @"Console.WriteLine(" + extractStringParams (a,c) + ");"
    | _ -> ""

let buildCode (nodes:Node<string> list) = 
    List.fold (fun acc node -> acc + transpileNode node ) "" nodes

let generateCodeFile (nodes:Node<string> list) =
    String.Format(generateBoilerPlate, buildCode nodes)

let readLines (filePath:string) =
    List.ofSeq(System.IO.File.ReadLines(filePath))

let (|Prefix|_|) (p:string) (s:string) =
    if s.StartsWith(p) then
        Some(s.Substring(p.Length))
    else
        None

let buildNode line =
    match line with
    | Prefix "Display" x -> [Node(Node(Leaf, x.Trim(), Leaf), "Display", Leaf)]
    | _ -> []

let rec traverse line nodes =
    match line with
    | [] -> nodes
    | first::rest -> 
        traverse rest (List.concat [buildNode first;nodes])

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    let lines = readLines @"C:\temp\main.ns"
    let a = traverse lines [] |> generateCodeFile
    System.Console.ReadKey() |> ignore
    0 // return an integer exit code
