// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System;

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

let buildCode nodes = 
    ""

let generateCodeFile nodes =
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
