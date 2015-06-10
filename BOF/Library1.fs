namespace BOF

type Class1() = 
    member this.X = "F#"

type X = float
type Y = float
type Coordinate = X * Y

type Q = int
type R = int
type S = int
type Hex =
    {
        q : Q
        r : R
        s : S
    }
    static member (+) (a:Hex, b:Hex) =
        {
            q = a.q + b.q
            r = a.r + b.r
            s = a.s + b.s
        }
    static member (-) (a:Hex, b:Hex) =
        {
            q = a.q - b.q
            r = a.r - b.r
            s = a.s - b.s
        }
    static member Scale (a:Hex, k:int) =
        {
            q = a.q * k
            r = a.r * k
            s = a.s * k
        }

module HexConstants =
    [<Literal>]
    let Directions = [(1,0,-1); (1,-1,0); (0,-1,1); (-1,0,1); (-1,1,0); (0,1,-1)]
        


    
    



