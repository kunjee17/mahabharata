(**
Lesson 1 - Basic understanding of NLP. Getting started with term,document frequency for term and Inverse document frequency for term. 
*)
#I "../packages"
#r "Microsoft.FsharpLu.Json/lib/net452/Microsoft.FsharpLu.Json.dll"
#r "Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#r "MathNet.Numerics/lib/net40/MathNet.Numerics.dll"
#r "MathNet.Numerics.FSharp/lib/net40/MathNet.Numerics.FSharp.dll"

open System.IO
open System
open System.Text.RegularExpressions
open Newtonsoft.Json
open Microsoft.FSharpLu.Json
open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.SparseMatrix


type DocItem = {
    Term : string 
    Freq : int
}

type Doc = {
    Bookno : string
    DocItems : DocItem []
}


type Book(bookno:string) = 

    //should be done with regex. But I don't know regex so I copy pasted from SO to remove all special chars
    let removeSpecialChars (str : string) = Regex.Replace(str, "[^0-9a-zA-Z]+", " ")

    member x.BookNo = bookno
    member x.BookText = 
                Path.Combine(__SOURCE_DIRECTORY__, "..", "books/"+ bookno + ".txt")
                |> File.ReadAllLines

    member x.TermFrequency (input:string) = 
            input
            // |> String.concat " " 
            |> (fun x -> x.Split ' ')
            |> Array.map (removeSpecialChars >> (fun x -> x.Trim())) 
            |> Array.filter (fun x -> x <> "")
            |> Array.countBy id
    member x.BookTermGroup = 
            x.TermFrequency (x.BookText |> String.concat " ")
             
            

    member x.BookUniqueTerms = 
        x.BookTermGroup 
            |> Array.map (fun (x,_)-> x)
            |> set


let booknos = [|"01";"02";"03";"04";"05";"06";"07";"08";"09";"10";"11";"12";"13";"14";"15";"16";"17";"18"|]

let books = [|
    for n in booknos do
        yield Book(n) 
|]


let (m : Matrix<float>) = 
    let allbookTerms = books 
                        |> Array.map (fun x -> x.BookUniqueTerms) 
                        |> Array.fold (+) Set.empty
    let sm = SparseMatrix.zero books.Length allbookTerms.Count
    let book0 = books.[0]
    allbookTerms 
        |> Set.toArray
        |> Array.iteri (fun i x  -> 
                            if book0.BookUniqueTerms.Contains x then  sm.[0,i]<- 1.0 
                        )
    sm

let getUniqueTermsByFrequencyForBook (bookno : string) : Doc = 
    let bookToProcess = books |> Array.find (fun x -> x.BookNo = bookno)
    let allOtherBooks = books |> Array.filter (fun x -> x.BookNo <> bookno)

    let allBookTerms = allOtherBooks |> Array.map (fun x -> x.BookUniqueTerms) |> Array.fold (+) Set.empty

    let termsSpecifictoBook = bookToProcess.BookUniqueTerms - allBookTerms
    let docItems: DocItem [] = bookToProcess.BookTermGroup 
                                |> Array.filter (fun (x,_) -> termsSpecifictoBook.Contains x) 
                                |> Array.map (fun (x,y)-> { Term = x; Freq = y})
    {Bookno = bookno; DocItems = docItems}
    

let uniqueTermsforAllBooksAsDoc = booknos |> Array.map getUniqueTermsByFrequencyForBook 

let uniqueTermsforAllBooks = books |> Array.map (fun x -> x.BookUniqueTerms) |> Array.fold (fun acc elem -> acc + elem) Set.empty

let invertedIndex (term:string) =
    let n = books |> Array.filter (fun x -> x.BookUniqueTerms.Contains term) |> Array.length |> float
    let m = books |> Array.length |> float
    log (m/n)


let invertedIndexTerms = 
    uniqueTermsforAllBooks 
        |> Set.map (fun x -> (x,invertedIndex x))

// let terms =
//     Compact.serialize uniqueTermsforAllBooksAsDoc 

// let JsonToFile (path:string)(text : string) = 
//     use writer = new StreamWriter(path)
//     writer.WriteLine ("var terms=" + text)

// let jsonpath = Path.Combine(__SOURCE_DIRECTORY__, "..", "docs/js/" + "terms" + ".js")

// JsonToFile jsonpath terms



