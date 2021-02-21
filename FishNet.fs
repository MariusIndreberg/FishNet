module monoTest.FishNet
open Microsoft.Xna.Framework
open System
open Microsoft.Xna.Framework.Graphics

type Temperature = Temperature of float 

type Salinity = Salinity of float 

type Density  = Density of float 

type Component =
    | Position of Vector2
    | Temperature of Temperature
    | Salinity of Salinity 
    | Density of Density 

type Entity = {
    Id : Guid 
}

type InputData = {
    SeaWaterVelocity        : Vector3 
}

module InputData =  
    let New sv  = {
        SeaWaterVelocity = sv 
    }

let testSimulationMap =   
    Map.empty 
    |> Map.add 0 (InputData.New <| Vector3(-5f, -5f, 0f))
    |> Map.add 1 (InputData.New <| Vector3(4f, -3f, 0f))
    |> Map.add 2 (InputData.New <| Vector3(2f, -1f, 2f))
    |> Map.add 3 (InputData.New <| Vector3(-0f, 0.0f, 3f))
    |> Map.add 4 (InputData.New <| Vector3(-3f, -2f, 1f))

[<Literal>]
let BoundX = 800 

[<Literal>]
let BoundY = 600

[<Struct>]
type Particle = {
    Uuid        : Guid
    Pos         : Vector2 
    Z           : float32 
}
module Particle = 
    let New pos = {
        Uuid        = Guid.NewGuid()
        Pos         = pos 
        Z           = 100f
    } 

let private updatePosition step particle = 
    let data = testSimulationMap.[step]
    let pos = Vector2(particle.Pos.X + data.SeaWaterVelocity.X, particle.Pos.Y + data.SeaWaterVelocity.Y)
    { particle with  Pos = pos; } 

let private updateZ step particle = 
    let data = testSimulationMap.[step]
    let z = particle.Z + data.SeaWaterVelocity.Z
    { particle with  Z = z } 

let advectOcean (step : int) (particles : Particle []) = 
    particles 
    |> Array.Parallel.map (updatePosition step)

let advectZ (step : int) (particles : Particle []) = 
    particles 
    |> Array.Parallel.map (updateZ step)

type ViewMode = 
    | TopDown
    | Side

type Simulation = {
    Particles   : Particle []
    CurrentStep : int
    ViewMode    : ViewMode 
}
module Simulation = 
    let Init (n : int) = 
        {   
            CurrentStep = 0
            Particles   = Array.init n (fun x -> 
                let pos = Vector2(400f, 300f)
                Particle.New pos ) 
            ViewMode = TopDown
        }

    let Update (s : Simulation) (gt : GameTime ) =
        s.Particles
        |> advectOcean s.CurrentStep
        |> advectZ s.CurrentStep
        |> fun particles -> { s with CurrentStep = s.CurrentStep + 1; Particles = particles }

    let private drawTopDown (s : Simulation) (tex : Texture2D) (sb : SpriteBatch) =
        s.Particles
        |> Array.iter (fun p -> sb.Draw(tex, p.Pos, Color.White) )

    let private drawSide (s : Simulation) (tex : Texture2D) (sb : SpriteBatch) = 
        s.Particles
        |> Array.iter (fun p -> sb.Draw(tex, Vector2(p.Pos.X, float32 p.Z), Color.White) )

    let ChangeMode (s : Simulation) (mode : ViewMode) = 
        { s with ViewMode = mode }

    let Draw (s : Simulation) (tex : Texture2D) (sb : SpriteBatch) (gameTime : GameTime) = 
        match s.ViewMode with 
        | TopDown -> drawTopDown s tex sb 
        | Side -> drawSide s tex sb