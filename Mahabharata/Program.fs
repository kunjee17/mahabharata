
[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    
    System.Console.Read() |> ignore
    0 // return an integer exit code
