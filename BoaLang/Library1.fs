//module BoaLang
//
//open FParsec
//
//type label = string
//type identifier = string
//type index = int
//type HashTable<'k,'v> = System.Collections.Generic.Dictionary<'k,'v>
///// Small Basic arithmetic operation
//type arithmetic = Add | Subtract | Multiply | Divide
///// Small Basic comparison operaton
//type comparison = Eq | Ne | Lt | Gt | Le | Ge
///// Small Basic logical operation
//type logical = And | Or
///// Small Basic value
//type value =
//    | Bool of bool
//    | Int of int
//    | Double of double
//    | String of string
//    | Array of HashTable<value,value>
//
//let test p str =
//    match run p str with
//    | Success(result, _, _) -> printfn "Success: %A" result
//    | Failure(errorMessage, _, _) -> printfn "Failure: %s" errorMessage
//
//let pnumvalue: Parser<value, unit> =
//    let numberFormat = NumberLiteralOptions.AllowFraction
//    numberLiteral numberFormat "number"
//    |>> fun nl ->
//            if nl.IsInteger then Int (int nl.String)
//            else Double(float nl.String)
//
//let ws = skipManySatisfy (fun c -> c = ' ' || c = '\t' || c='\r') // spaces
//let str_ws s = pstring s .>> ws
//let str_ws1 s = pstring s .>> spaces1
//let strCI_ws c = pstringCI c .>> ws
//
//let pstringvalue = 
//    between (pstring "\"") (pstring "\"") (manySatisfy (fun x -> x <> '"')) 
//    |>> (fun s -> String(s))
//
//let pvalue = pnumvalue <|> pstringvalue
//
//let intNumber = pint32 .>> ws
//
//let doubleQuoteStringLiteral = between (pstring "\"") (pstring "\"" ) (manySatisfy ((<>) '\"' ))
//let singleQuoteStringLiteral = between (pstring "\'") (pstring "\'" ) (manySatisfy ((<>) '\'' ))
//   
//let stringLiteral = doubleQuoteStringLiteral <|> singleQuoteStringLiteral
//
//// Tokens
//let include : Parser<string, unit> = strCI_ws "include"
//
//
//
//
//
//
//
//
//
