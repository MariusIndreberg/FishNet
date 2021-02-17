module monoTest.Config

type Diffusitivity = 
    | Environment 
    | StepFunction 
    | WindSpeedSundBy83 
    | WindSpeedLarge1994
    | GlsTke 
    | Constant

type StokesDriftFetch = 
    | FiveThousand 
    | TwentyFiveThousand
    | FiftyThousand


// Advect elements with vertical component of ocean current.
type VerticalAdvection = VerticalAdvection of bool

// Activate vertical mixing scheme with inner loop'
type VerticalMixing = VerticalMixing of bool 

// 'Time step used for inner loop of vertical mixing.
type VerticalMixingTimeStep = VerticalMixingTimeStep of float

// Algorithm/source used for profile of vertical diffusivity. Environment means that diffusivity is aquired from readers or environment constants/fallback.
type VerticalMixingDiffusitivity = VerticalMixingDiffusitivity of Diffusitivity

// Update T and S profiles within inner loop of vertical mixing. This takes more time, but may be slightly more accurate
type VerticalMixingTSProfile = VerticalMixingTSProfile of bool 

// 'The direct wind drift (windage) is linearly decreasing from the surface value (wind_drift_factor) until 0 at this depth.',
type WindDriftDepth = WindDriftDepth of float 

// 'Advection elements with Stokes drift (wave orbital motion).',
type StokesDrift = StokesDrift of bool

//'If True, Stokes drift is estimated from wind based on look-up-tables for given fetch (drift:tabularised_stokes_drift_fetch).',
type UseTabulurizedStokesDrift =  UseTabulurizedStokesDrift of bool 


//The fetch length when using tabularised Stokes drift.
type TabularisedStokesDriftFetch = TabularisedStokesDriftFetch of StokesDriftFetch

//If True, elements hitting/penetrating seafloor, are lifted to seafloor height. The alternative (False) is to deactivate elements).
type LiftToSeaFloor = LiftToSeaFloor of bool

//'Ocean model data are only read down to at most this depth, and extrapolated below. May be specified to read less data to improve performance.
type TruncateOceanModelBelowM = TruncateOceanModelBelowM of float 

//Depth below sea level where elements are released. This depth is neglected if seafloor seeding is set selected.
type SeedZ = SeedZ of float 

//Elements are seeded at seafloor, and seeding depth (z) is neglected.
type SeedSeaFloor = SeedSeaFloor of bool 


type Config = {
    VerticalAdvection           : VerticalAdvection
    VerticalMixing              : VerticalMixing
    VerticalMixingDiffusitivity : VerticalMixingDiffusitivity
    VerticalMixingTimeStep      : VerticalMixingTimeStep
    WindDriftDepth              : WindDriftDepth
    StokesDrift                 : StokesDrift 
    UseTabulurizedStokesDrift   : UseTabulurizedStokesDrift
    TabularisedStokesDriftFetch : TabularisedStokesDriftFetch
    LiftToSeaFloor              : LiftToSeaFloor
    TruncateOceanModelBelowM    : TruncateOceanModelBelowM
    SeedZ                       : SeedZ 
    SeedSeaFloor                : SeedSeaFloor
}
