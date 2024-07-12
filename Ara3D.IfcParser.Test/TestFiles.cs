    using Ara3D.Utils;

namespace Ara3D.IfcParser.Test;

public static class TestFiles
{
    // < 2MB
    public static FilePath[] TinyFiles() => new FilePath[]
    {
            // 0B
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-994201.ifc",
// 0B
            @"C:\Users\cdigg\dev\impraria\Trojena\101050 - THE VAULT\07-020100-0000101050-SBP-STR-BIM-010003.ifc",
// 0B
            @"C:\Users\cdigg\dev\impraria\Trojena\101050 - THE VAULT\EXPORTED IFC\LAV-ARC-BIM-002030_IFC.ifc",
// 6.4KB
            @"C:\Users\cdigg\dev\impraria\NIC Logistics Warehouse\03-230000-0000100120-DAH-ENV-MDL-450001_A.ifc",
// 6.4KB
            @"C:\Users\cdigg\dev\impraria\NIC Logistics Warehouse\03-230000-0000100120-DAH-ENV-MDL-450002_A.ifc",
// 6.4KB
            @"C:\Users\cdigg\dev\impraria\NIC Logistics Warehouse\03-230000-0000100120-DAH-ENV-MDL-450003_A.ifc",
// 6.4KB
            @"C:\Users\cdigg\dev\impraria\NIC Logistics Warehouse\03-230000-0000100120-DAH-ENV-MDL-450004_A.ifc",
// 6.8KB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\FPR\07-186100-4800000333-CH2-FPR-MDL-994201.ifc",
// 6.9KB
            @"C:\Users\cdigg\dev\impraria\Trojena\100120-073 - NEOM MOUNTAIN - BASECAMP\IFC FORMAT\07-620000-0000100120-DAH-TRN-MDL-973006_A.ifc",
// 7.0KB
            @"C:\Users\cdigg\dev\impraria\Trojena\100120-073 - NEOM MOUNTAIN - BASECAMP\IFC FORMAT\07-620000-0000100120-DAH-TRN-MDL-973007_A.ifc",
// 8.4KB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000000_IFC_B.ifc",
// 9.2KB
            @"C:\Users\cdigg\dev\impraria\02 - Gulf of Aqaba\4800000194 - GOA Romantic Bay\Stage 3A\IFC\ARC\02-211211-4800000194-WBP-ARC-MDL-000000.ifc",
// 10.6KB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-900401_A.ifc",
// 11.6KB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000010_IFC_B.ifc",
// 201.1KB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A\IFC\MEP\07-004003-4200000004-AED-MEP-MDL-006200.ifc",
// 221.2KB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-990006_IFC_A.ifc",
// 228.2KB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000001_IFC_B.ifc",
// 229.9KB
            @"C:\Users\cdigg\dev\impraria\NIC Logistics Warehouse\03-230000-0000100120-DAH-TRN-MDL-450001_A.ifc",
// 513.5KB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\MEP\07-004003-4200000004-AED-MEP-MDL-006106_IFC_A.ifc",
// 528.4KB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\SGN\07-004003-4200000004-AED-SGN-MDL-000015_IFC_A.ifc",
// 528.9KB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\SGN\07-004003-4200000004-AED-SGN-MDL-000018_IFC_A.ifc",
// 627.9KB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\SGN\07-004003-4200000004-AED-SGN-MDL-000019_IFC_A.ifc",
// 722.3KB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\SGN\07-004003-4200000004-AED-SGN-MDL-000016_IFC_A.ifc",
// 909.8KB
            @"C:\Users\cdigg\dev\impraria\Trojena\4200000004 - NEOM SKI VILLAGE CONCEPT DESIGN\07-004003-4200000004-AED-STR-MDL-006100.ifc",
// 909.8KB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A\IFC\STR\07-004003-4200000004-AED-STR-MDL-006100.ifc",
// 1.1MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\MEP\07-004003-4200000004-AED-MEP-MDL-006105_IFC_A.ifc",
// 1.3MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-546102-4800000333-CH2-ARC-MDL-900305_A.ifc",
// 1.3MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-266101-4800000333-CH2-ARC-MDL-900356_A.ifc",
// 1.4MB
            @"C:\Users\cdigg\dev\impraria\Trojena\100120-073 - NEOM MOUNTAIN - BASECAMP\IFC FORMAT\07-620000-0000100120-DAH-INF-MDL-973100_A.ifc",
// 1.4MB
            @"C:\Users\cdigg\dev\impraria\Trojena\101050 - THE VAULT\07-020100-0000101050-SBP-STR-BIM-020001.ifc",
// 1.4MB
            @"C:\Users\cdigg\dev\impraria\Trojena\4200000000-002 - NEOM MOUNTAIN PACKAGE 2 ACCESS ROAD\IFC - EXPORTED FOR 3DS\07-141102-4200000000-ZFP-CIV-MDL-100005_A.ifc",
// 1.5MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A\IFC\STR\07-004003-4200000004-AED-STR-MDL-000300.ifc",
// 1.5MB
            @"C:\Users\cdigg\dev\impraria\Trojena\100120-073 - NEOM MOUNTAIN - BASECAMP\IFC FORMAT\07-620000-0000100120-DAH-INF-MDL-973200_A.ifc",
// 1.6MB
            @"C:\Users\cdigg\dev\impraria\Trojena\101050 - THE VAULT\07-020100-0000101050-SBP-STR-BIM-020002.ifc",
// 1.7MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\SGN\07-004003-4200000004-AED-SGN-MDL-000017_IFC_A.ifc",
// 1.7MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ELE\07-186100-4800000333-CH2-ELE-MDL-994201.ifc",
// 1.8MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ELE\07-004003-4200000004-AED-ELE-MDL-006101_IFC_A.ifc",
// 1.9MB
            @"C:\Users\cdigg\dev\impraria\Trojena\101050 - THE VAULT\07-020100-0000101050-SBP-STR-BIM-020004.ifc",
    };


    // 2 to 10MB
    public static FilePath[] SmallFiles() => new FilePath[]
    {
            // 2.4MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\INF\07-004003-4200000004-AED-INF-MDL-006101_IFC_A.ifc",
// 2.5MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-990001_IFC_C.ifc",
// 2.8MB
            @"C:\Users\cdigg\dev\impraria\Trojena\100120-073 - NEOM MOUNTAIN - BASECAMP\IFC FORMAT\07-620000-0000100120-DAH-INF-MDL-973300_A.ifc",
// 2.8MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000030_IFC_B.ifc",
// 2.9MB
            @"C:\Users\cdigg\dev\impraria\Trojena\100120-073 - NEOM MOUNTAIN - BASECAMP\IFC FORMAT\07-620000-0000100120-DAH-INF-MDL-973400_A.ifc",
// 2.9MB
            @"C:\Users\cdigg\dev\impraria\Trojena\100120-073 - NEOM MOUNTAIN - BASECAMP\IFC FORMAT\07-620000-0000100120-DAH-TRN-MDL-973005_A.ifc",
// 2.9MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-546101-4800000333-CH2-ARC-MDL-900361_A.ifc",
// 2.9MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000025_IFC_B.ifc",
// 3.0MB
            @"C:\Users\cdigg\dev\impraria\Trojena\4200000000-002 - NEOM MOUNTAIN PACKAGE 2 ACCESS ROAD TUNNEL\ARC-MDL-000001_A.ifc",
// 3.0MB
            @"C:\Users\cdigg\dev\impraria\02 - Gulf of Aqaba\4800000194 - GOA Romantic Bay\Stage 3A\IFC\IDN\02-211211-4800000194-WBP-IDN-MDL-000001.ifc",
// 3.3MB
            @"C:\Users\cdigg\dev\impraria\100517_NEOM Camp 200\01-620400-0000100517-NAP-STR-MDL-000003_IFC.ifc",
// 3.3MB
            @"C:\Users\cdigg\dev\impraria\02 - Gulf of Aqaba\4800000194 - GOA Romantic Bay\Stage 3A\IFC\SEC\02-211211-4800000194-WBP-SEC-MDL-000001.ifc",
// 3.5MB
            @"C:\Users\cdigg\dev\impraria\NIC Logistics Warehouse\03-230000-0000100120-DAH-STR-MDL-450001_A.ifc",
// 3.6MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-990002_IFC_A.ifc",
// 3.6MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\INF\07-004003-4200000004-AED-INF-MDL-006103_IFC_A.ifc",
// 3.9MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-900320_A.ifc",
// 4.0MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000026_IFC_B.ifc",
// 4.1MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-990003_IFC_A.ifc",
// 4.5MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\MEP\07-004003-4200000004-AED-MEP-MDL-006104_IFC_A.ifc",
// 4.6MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000021_IFC_B.ifc",
// 4.6MB
            @"C:\Users\cdigg\dev\impraria\Trojena\4200000004 - NEOM SKI VILLAGE CONCEPT DESIGN\07-004003-4200000004-AED-SKI-MDL-000001_A.ifc",
// 4.6MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A\IFC\MPL\07-004003-4200000004-AED-SKI-MDL-000001_A.ifc",
// 4.8MB
            @"C:\Users\cdigg\dev\impraria\02 - Gulf of Aqaba\4800000194 - GOA Romantic Bay\Stage 3A\IFC\LAN\02-211211-4800000194-WBP-LAN-MDL-000001.ifc",
// 5.2MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-990004_IFC_A.ifc",
// 5.3MB
            @"C:\Users\cdigg\dev\impraria\Trojena\4200000004 - NEOM SKI VILLAGE CONCEPT DESIGN\07-004003-4200000004-AED-ARC-MDL-000002_A.ifc",
// 5.3MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000002_A.ifc",
// 5.7MB
            @"C:\Users\cdigg\dev\impraria\Trojena\101050 - THE VAULT\07-020100-0000101050-SBP-STR-BIM-020003.ifc",
// 5.8MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-900370_A.ifc",
// 5.8MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-216102-4800000333-CH2-ARC-MDL-900312_A.ifc",
// 6.1MB
            @"C:\Users\cdigg\dev\impraria\100517_NEOM Camp 200\01-620400-0000100517-NAP-CIV-MDL-000002_IFC.ifc",
// 6.1MB
            @"C:\Users\cdigg\dev\impraria\Trojena\4200000000-002 - NEOM MOUNTAIN PACKAGE 2 ACCESS ROAD\IFC - EXPORTED FOR 3DS\07-141102-4200000000-ZFP-CIV-MDL-100003_A.ifc",
// 6.8MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\SKI\07-004003-4200000004-AED-SKI-MDL-006101_IFC_C.ifc",
// 7.1MB
            @"C:\Users\cdigg\dev\impraria\02 - Gulf of Aqaba\4800000194 - GOA Romantic Bay\Stage 3A\IFC\MEP\02-211211-4800000194-WBP-MEP-MDL-000001.ifc",
// 7.4MB
            @"C:\Users\cdigg\dev\impraria\NIC Logistics Warehouse\03-230000-0000100120-DAH-LAN-MDL-450001_A.ifc",
// 7.9MB
            @"C:\Users\cdigg\dev\impraria\Trojena\100120-073 - NEOM MOUNTAIN - BASECAMP\IFC FORMAT\07-620000-0000100120-DAH-TRN-MDL-973001_A.ifc",
// 8.0MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-546102-4800000333-CH2-ARC-MDL-900355_A.ifc",
// 8.1MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\CIV\07-004003-4200000004-AED-CIV-MDL-006201_IFC_C.ifc",
// 8.9MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000001_A.ifc",
// 8.9MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000013_IFC_B.ifc",
// 9.2MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000027_IFC_B.ifc",
// 9.3MB
            @"C:\Users\cdigg\dev\impraria\Trojena\101050 - THE VAULT\07-020100-0000101050-GEG-WMN-BIM-010001.ifc",
// 9.4MB
            @"C:\Users\cdigg\dev\impraria\Trojena\100120-073 - NEOM MOUNTAIN - BASECAMP\IFC FORMAT\07-620000-0000100120-DAH-TRN-MDL-973003_A.ifc",
// 9.5MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A\IFC\STR\07-004003-4200000004-AED-STR-MDL-100000.ifc",
// 9.6MB
            @"C:\Users\cdigg\dev\impraria\NIC Logistics Warehouse\03-230000-0000100120-DAH-ICT-MDL-450001_A.ifc",
    };

    // 10 to 50
    public static FilePath[] MediumFiles() => new FilePath[]
    {

// 10.7MB
            @"C:\Users\cdigg\dev\impraria\Trojena\101050 - THE VAULT\07-020100-0000101050-SBP-STR-BIM-010004.ifc",
// 10.8MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000018_IFC_B.ifc",
// 11.7MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-216101-4800000333-CH2-ARC-MDL-900301_A.ifc",
// 11.9MB
            @"C:\Users\cdigg\dev\impraria\Trojena\4200000000-002 - NEOM MOUNTAIN PACKAGE 2 ACCESS ROAD\IFC - EXPORTED FOR 3DS\07-141102-4200000000-ZFP-CIV-MDL-100007_A.ifc",
// 12.0MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-546101-4800000333-CH2-ARC-MDL-900309_A.ifc",
// 12.4MB
            @"C:\Users\cdigg\dev\impraria\Trojena\100120-073 - NEOM MOUNTAIN - BASECAMP\IFC FORMAT\07-620000-0000100120-DAH-TRN-MDL-973004_A.ifc",
// 12.9MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-536102-4800000333-CH2-ARC-MDL-900306_A.ifc",
// 13.2MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-546102-4800000333-CH2-ARC-MDL-900357_A.ifc",
// 13.6MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A\IFC\STR\07-004003-4200000004-AED-STR-MDL-000100.ifc",
// 14.8MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000012_IFC_B.ifc",
// 14.9MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-900571_A.ifc",
// 15.1MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-536102-4800000333-CH2-ARC-MDL-900358_A.ifc",
// 15.4MB
            @"C:\Users\cdigg\dev\impraria\Trojena\100120-073 - NEOM MOUNTAIN - BASECAMP\IFC FORMAT\07-620000-0000100120-DAH-TRN-MDL-973002_A.ifc",
// 15.7MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-900551_A.ifc",
// 15.8MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000015_IFC_B.ifc",
// 15.9MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186101-4800000333-CH2-ARC-MDL-900210_A.ifc",
// 16.1MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000032_IFC_B.ifc",
// 16.7MB
            @"C:\Users\cdigg\dev\impraria\Trojena\4200000000-002 - NEOM MOUNTAIN PACKAGE 2 ACCESS ROAD\IFC - EXPORTED FOR 3DS\07-141102-4200000000-ZFP-STR-MDL-000001_A.ifc",
// 17.3MB
            @"C:\Users\cdigg\dev\impraria\Trojena\101050 - THE VAULT\07-020100-0000101050-SBP-STR-BIM-020005.ifc",
// 19.1MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-900501_A.ifc",
// 20.1MB
            @"C:\Users\cdigg\dev\impraria\0000100120-093 - OXAGON ADVANCED HEALTH CENTER\STAGE 3A - CONCEPT DESIGN\ELE\03-730000-0000100120-DAH-ELE-MDL-000009_IFC_C.ifc",
// 20.2MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-216102-4800000333-CH2-ARC-MDL-900362_A.ifc",
// 20.7MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\MEC\07-186100-4800000333-CH2-MEC-MDL-994201.ifc",
// 22.1MB
            @"C:\Users\cdigg\dev\impraria\100517_NEOM Camp 200\01-620400-0000100517-NAP-ARC-MDL-000003_IFC.ifc",
// 22.4MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000019_IFC_B.ifc",
// 23.4MB
            @"C:\Users\cdigg\dev\impraria\0000100120-093 - OXAGON ADVANCED HEALTH CENTER\STAGE 3A - CONCEPT DESIGN\ELV\03-730000-0000100120-DAH-ELV-MDL-000009_IFC_C.ifc",
// 25.2MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\MEP\07-004003-4200000004-AED-MEP-MDL-006101_IFC_A.ifc",
// 26.7MB
            @"C:\Users\cdigg\dev\impraria\100517_NEOM Camp 200\01-620400-0000100517-NAP-STR-MDL-000002_IFC.ifc",
// 27.8MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000020_IFC_B.ifc",
// 29.8MB
            @"C:\Users\cdigg\dev\impraria\NIC Logistics Warehouse\03-230000-0000100120-DAH-ARC-MDL-450001_A.ifc",
// 30.7MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000011_IFC_B.ifc",
// 30.8MB
            @"C:\Users\cdigg\dev\impraria\NIC Logistics Warehouse\03-230000-0000100120-DAH-ARC-MDL-450002_A.ifc",
// 31.1MB
            @"C:\Users\cdigg\dev\impraria\NIC Logistics Warehouse\03-230000-0000100120-DAH-MEC-MDL-450001_A.ifc",
// 31.5MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-900403_A.ifc",
// 32.5MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-900402_A.ifc",
// 34.5MB
            @"C:\Users\cdigg\dev\impraria\100517_NEOM Camp 200\01-620400-0000100517-NAP-ARC-MDL-000002_IFC.ifc",
// 35.2MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-900351_A.ifc",
// 35.9MB
            @"C:\Users\cdigg\dev\impraria\Trojena\101050 - THE VAULT\07-020100-0000101050-SBP-STR-BIM-010001.ifc",
// 37.7MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\INF\07-004003-4200000004-AED-INF-MDL-006102_IFC_A.ifc",
// 38.1MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\MEP\07-004003-4200000004-AED-MEP-MDL-006103_IFC_A.ifc",
// 40.5MB
            @"C:\Users\cdigg\dev\impraria\100517_NEOM Camp 200\01-620400-0000100517-NAP-CIV-MDL-000001_IFC.ifc",
// 41.4MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-900402_A.ifc",
// 42.0MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000016_IFC_B.ifc",
// 45.6MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-900401_A.ifc",
// 47.0MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-900406_A.ifc",
// 47.4MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ELE\07-004003-4200000004-AED-ELE-MDL-000001_IFC_D.ifc",
// 47.7MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\FPR\07-004003-4200000004-AED-FPR-MDL-000001_IFC_D.ifc",
// 48.3MB
            @"C:\Users\cdigg\dev\impraria\100517_NEOM Camp 200\01-620400-0000100517-NAP-MEP-MDL-000002_IFC.ifc",
// 49.7MB
            @"C:\Users\cdigg\dev\impraria\02 - Gulf of Aqaba\4800000194 - GOA Romantic Bay\Stage 3A\IFC\ARC\02-211211-4800000194-WBP-ARC-MDL-000003.ifc",
    };

    // 50 to 150
    public static FilePath[] LargeFiles() => new FilePath[]
    {
// 50.2MB
            @"C:\Users\cdigg\dev\impraria\02 - Gulf of Aqaba\4800000194 - GOA Romantic Bay\Stage 3A\IFC\STR\02-211211-4800000194-WBP-STR-MDL-000001.ifc",
// 52.8MB
            @"C:\Users\cdigg\dev\impraria\Trojena\101050 - THE VAULT\07-020100-0000101050-SBP-STR-BIM-010002.ifc",
// 54.8MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\MEC\07-004003-4200000004-AED-MEC-MDL-006101_IFC_D.ifc",
// 57.3MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\TRN\07-004003-4200000004-AED-TRN-MDL-006101_IFC_C.ifc",
// 60.5MB
            @"C:\Users\cdigg\dev\impraria\100517_NEOM Camp 200\01-620400-0000100517-NAP-MEP-MDL-000003_IFC.ifc",
// 63.4MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\MEP\07-004003-4200000004-AED-MEP-MDL-006102_IFC_A.ifc",
// 64.0MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-994201.ifc",
// 64.2MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-900601_A.ifc",
// 66.2MB
            @"C:\Users\cdigg\dev\impraria\Trojena\4200000000-002 - NEOM MOUNTAIN PACKAGE 2 ACCESS ROAD\IFC - EXPORTED FOR 3DS\07-141102-4200000000-ZFP-CIV-MDL-100001_.ifc",
// 70.8MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-900603_A.ifc",
// 71.1MB
            @"C:\Users\cdigg\dev\impraria\NIC Logistics Warehouse\03-230000-0000100120-DAH-ELE-MDL-450001_A.ifc",
// 71.8MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-900602_A.ifc",
// 72.0MB
            @"C:\Users\cdigg\dev\impraria\Trojena\4200000004 - NEOM SKI VILLAGE CONCEPT DESIGN\07-004003-4200000004-AED-STR-MDL-000200.ifc",
// 72.0MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A\IFC\STR\07-004003-4200000004-AED-STR-MDL-000200.ifc",
// 72.1MB
            @"C:\Users\cdigg\dev\impraria\Trojena\4200000004 - NEOM SKI VILLAGE CONCEPT DESIGN\07-004003-4200000004-AED-FAC-MDL-000001_A.ifc",
// 72.1MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A\IFC\FAC\07-004003-4200000004-AED-FAC-MDL-000001_A.ifc",
// 84.4MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-000017_IFC_B.ifc",
// 88.5MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\SKI\07-004003-4200000004-AED-SKI-MDL-000001_IFC_C.ifc",
// 88.8MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-900650_A.ifc",
// 92.5MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\PLD\07-186100-4800000333-CH2-PLD-MDL-994201.ifc",
// 93.9MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\ARC\07-004003-4200000004-AED-ARC-MDL-200000_IFC_B.ifc",
// 102.3MB
            @"C:\Users\cdigg\dev\impraria\Trojena\4700000200 - NEOM LAKE CONCEPT DESIGN\07-020301-0000101049-BP-ARC-IFC-000001.ifc",
// 102.9MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-900405_A.ifc",
// 114.2MB
            @"C:\Users\cdigg\dev\impraria\NIC Logistics Warehouse\03-230000-0000100120-DAH-STR-MDL-450002_A.ifc",
// 115.0MB
            @"C:\Users\cdigg\dev\impraria\Trojena\4200000004 - NEOM SKI VILLAGE CONCEPT DESIGN\07-004003-4200000004-AED-MEC-MDL-006100.ifc",
// 115.0MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A\IFC\MEC\07-004003-4200000004-AED-MEC-MDL-006100.ifc",
// 128.6MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-900201_A.ifc",
// 129.4MB
            @"C:\Users\cdigg\dev\impraria\02 - Gulf of Aqaba\4800000194 - GOA Romantic Bay\Stage 3A\IFC\ARC\02-211211-4800000194-WBP-ARC-MDL-000001.ifc",
// 142.2MB
            @"C:\Users\cdigg\dev\impraria\0000100120-093 - OXAGON ADVANCED HEALTH CENTER\STAGE 3A - CONCEPT DESIGN\ARC\03-730000-0000100120-DAH-ARC-MDL-000019_IFC_C.ifc",
// 143.3MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-900301_A.ifc",
    };

    // 150 to 450
    public static FilePath[] VeryLargeFiles() => new FilePath[]
    {

// 156.2MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-900501_A.ifc",
// 163.9MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-900601_A.ifc",
// 182.6MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-900701_A.ifc",
// 183.0MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-900751_A.ifc",
// 199.0MB
            @"C:\Users\cdigg\dev\impraria\Trojena\101050 - THE VAULT\07-020100-0000101050-LAV-ARC-BIM-002010.ifc",
// 208.9MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-900404_A.ifc",
// 259.3MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-900701_A.ifc",
// 296.6MB
            @"C:\Users\cdigg\dev\impraria\0000100120-093 - OXAGON ADVANCED HEALTH CENTER\STAGE 3A - CONCEPT DESIGN\ARC\03-730000-0000100120-DAH-ARC-MDL-000009 _IFC_D.ifc",
// 343.6MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\STR\07-186100-4800000333-CH2-STR-MDL-990005_IFC_A.ifc",
// 374.2MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-900502_A.ifc",

    };

    public static FilePath[] HugeFiles() => new FilePath[]
        {
// 440.3MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\MEC\07-004003-4200000004-AED-MEC-MDL-000001_IFC_D.ifc",
// 498.9MB
            @"C:\Users\cdigg\dev\impraria\0000100120-093 - OXAGON ADVANCED HEALTH CENTER\STAGE 3A - CONCEPT DESIGN\IDN\03-730000-0000100120-DAH-IDN-MDL-000009_IFC_D.ifc",
// 699.4MB
            @"C:\Users\cdigg\dev\impraria\07 - NEOM Mountain\4200000004 - Ski Village\STAGE 3A 100%\IFC\PHE\07-004003-4200000004-AED-PHE-MDL-000001_IFC_D.ifc",
// 789.1MB
            @"C:\Users\cdigg\dev\impraria\IFC_selected_2023-05-30_06-47-30am\ARC\07-186100-4800000333-CH2-ARC-MDL-900201_A.ifc",
// 848.1MB
            @"C:\Users\cdigg\dev\impraria\02 - Gulf of Aqaba\4800000194 - GOA Romantic Bay\Stage 3A\IFC\KEQ\02-211211-4800000194-WBP-KEQ-MDL-000001.ifc",
        };

}