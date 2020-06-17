using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models {
    public class ExcelTemplate {

        public const string REKAPITULASI_PERDAGANGAN = "Equity\\Rekapitulasi Perdagngan.xlsx";
        public const string DERIVATIVE_KBIE = "Derivative\\KBIE_jatuh_tempo.xlsx";
        public const string DERIVATIVE_KBSUN = "Derivative\\KBSUN_jatuh_tempo.xlsx";
        public const string DUMPTOFILE_SECURITY = "Equity\\Dump to File\\Security.xlsx";
        public const string DUMPTOFILE_ISSUER = "Equity\\Dump to File\\Issuer.xlsx";
        public const string DUMPTOFILE_INDEX = "Equity\\Dump to File\\Index.xlsx";
        public const string DUMPTOFILE_INDEXMEMBER = "Equity\\Dump to File\\Raw\\Index Member.xlsx";
        public const string DUMPTOFILE_BOARD = "Equity\\Dump to File\\Board.xlsx";
        public const string DUMPTOFILE_RAW_SECURITY = "Equity\\Dump to File\\Raw\\Security.xlsx";
        public const string DUMPTOFILE_RAW_ISSUER = "Equity\\Dump to File\\Raw\\Issuer.xlsx";
        public const string DUMPTOFILE_RAW_INDEX = "Equity\\Dump to File\\Raw\\Index.xlsx";
        public const string DUMPTOFILE_RAW_INDEXMEMBER = "Equity\\Dump to File\\Raw\\Index Member.xlsx";
        public const string DUMPTOFILE_RAW_BOARD = "Equity\\Dump to File\\Raw\\Board.xlsx";
        public const string PELAPORAN_EVALUASI_NAR_TRANSAKSI_PS_NEGO = "Equity\\PelaporanEvaluasi\\RekapitulasiTransaksiNAR.xlsx";
        public const string PELAPORAN_EVALUASI_NAR_DAFTAR_AB = "Equity\\PelaporanEvaluasi\\DaftarABNAR.xlsx";
        public const string PELAPORAN_EVALUASI_NAR_REKAP_ALASAN = "Equity\\PelaporanEvaluasi\\AlasanTransaksiNAR.xlsx";
        public const string PELAPORAN_EVALUASI_NAR_GRAFIK_ALASAN = "Equity\\PelaporanEvaluasi\\GrafikAlasanNAR.xlsx";
        public const string PELAPORAN_EVALUASI_KOR_TRADING_ID = "Equity\\PelaporanEvaluasi\\PerbandinganKoreksiTradingID.xlsx";

        public const string PELAPORAN_EVALUASI_NAR_GRAFIK_HISTORI = "Equity\\PelaporanEvaluasi\\GrafikTransaksiNAR.xlsx";
        public const string EQUITY_FSM = "Equity\\Fsm.xlsx";
        public const string PELAPORAN_EVALUASI_NAR_TERBANYAK = "Equity\\PelaporanEvaluasi\\NARTerbanyak.xlsx";
        public const string PELAPORAN_EVALUASI_KOREKSI_TRADING_ID_TERBANYAK = "Equity\\PelaporanEvaluasi\\KoreksiTradingIDTerbanyak.xlsx";
        public const string PELAPORAN_EVALUASI_KOREKSI_TRADING_ID_REPORT = "Equity\\PelaporanEvaluasi\\KoreksiTradingIDReport.xlsx";
        public const string PELAPORAN_EVALUASI_NAR_REPORT = "Equity\\PelaporanEvaluasi\\NARReport.xlsx";

        public const string PELAPORAN_EVALUASI_INVALID_TRADING_ID_GRAFIK_HISTORIS = "Equity\\PelaporanEvaluasi\\InvalidTradingIDGrafikHistoris.xlsx";
        public const string PELAPORAN_EVALUASI_INVALID_TRADING_ID_REKAPITULASI = "Equity\\PelaporanEvaluasi\\InvalidTradingIDRekapitulasiInvalid.xlsx";
        public const string PELAPORAN_EVALUASI_INVALID_TRADING_ID_PERBANDINGAN = "Equity\\PelaporanEvaluasi\\InvalidTradingIDPerbandinganInvalid.xlsx";
        public const string PELAPORAN_EVALUASI_INVALID_TRADING_ID_TERBANYAK = "Equity\\PelaporanEvaluasi\\InvalidTradingIDABTerbanyak.xlsx";
        public const string PELAPORAN_EVALUASI_INVALID_TRADING_ID_MENCANTUMKAN = "Equity\\PelaporanEvaluasi\\InvalidTradingIDABMencantumkan.xlsx";
        public const string PELAPORAN_EVALUASI_INVALID_TRADING_ID_REPORTDATA = "Equity\\PelaporanEvaluasi\\InvalidTradingIDReportData.xlsx";
        
        //modul obligasi update parameter
        public const string OBLIGASI_FSM = "Obligasi\\ExportFSM\\Fsm.xlsx";
        public const string OBLIGASI_LISTING_COUPON_CSV = "Obligasi\\Listing\\CouponDetailCSV.xlsx";
        public const string OBLIGASI_LISTING_COUPON_DETAIL = "Obligasi\\Listing\\CouponDetailPLTE.xlsx";
        public const string OBLIGASI_LISTING_RATING_PLTE = "Obligasi\\Listing\\RatingPLTE.xlsx";
        public const string OBLIGASI_LISTING_DETAIL_FITS = "Obligasi\\Listing\\ListingDetailFITS.xlsx";
        public const string OBLIGASI_LISTING_DETAIL_PLTE = "Obligasi\\Listing\\ListingDetailPLTE.xlsx";
        public const string OBLIGASI_MATURITY_DETAIL_PLTE = "Obligasi\\Maturity\\MaturityPLTE.xlsx";
        public const string OBLIGASI_MATURITY_DETAIL_FITS = "Obligasi\\Maturity\\MaturityFITS.xlsx";
        public const string OBLIGASI_MATURITY_RATING_PLTE = "Obligasi\\Maturity\\MaturityRatingPLTE.xlsx";
        public const string OBLIGASI_ISSUER_FITS = "Obligasi\\ChangeInIssuer\\ChangeInIssuerFITS.xlsx";
        public const string OBLIGASI_ISSUER_PLTE = "Obligasi\\ChangeInIssuer\\ChangeInIssuerPLTE.xlsx";
        public const string OBLIGASI_CHANGE_RATING_FITS = "Obligasi\\ChangeInRating\\ChangeInRatingFITS.xlsx";
        public const string OBLIGASI_CHANGE_RATING_PLTE = "Obligasi\\ChangeInRating\\ChangeInRatingPLTE.xlsx";
        public const string OBLIGASI_CHANGE_RATING_PeRIODE_PLTE = "Obligasi\\ChangeInRatingPeriode\\changeinratingperiodePLTE.xlsx";
        public const string OBLIGASI_COUPON_UPDATE_FITS = "Obligasi\\CouponUpdate\\CouponUpdateFITS.xlsx";
        public const string OBLIGASI_COUPON_UPDATE_PLTE = "Obligasi\\CouponUpdate\\CouponUpdatePLTE.xlsx";
        public const string OBLIGASI_NOMINAL_ADJUSTMENT_FITS = "Obligasi\\NominalAdjustment\\NominalAdjustmentFITS.xlsx";
        public const string OBLIGASI_NOMINAL_ADJUSTMENT_PLTE = "Obligasi\\NominalAdjustment\\NominalAdjustmentPLTE.xlsx";
        public const string OBLIGASI_SUN_BENCHMARK = "Obligasi\\ChangeInSunBenchmark\\Benchmark.xlsx";
        public const string OBLIGASI_COUPON_PERIOD = "Obligasi\\ChangeCouponPeriod\\ChangeCouponPeriod.xlsx";
        public const string OBLIGASI_SUSPEND_ACTIVE = "Obligasi\\SuspendActive\\SuspendActive.xlsx";
        
        //modul obligasi setup parameter
        public const string OBLIGASI_MARKET_PARAMETER = "Obligasi\\MarketParameter\\MARKET_PARAMETER.xlsx";
        public const string OBLIGASI_ISSUER_PARAMETER = "Obligasi\\IssuerParameter\\ISSUER_PARAMETER.xlsx";
        public const string OBLIGASI_ISSUER_EBUS = "Obligasi\\IssuerEbus\\ISSUER_EBUS.xlsx";
        public const string OBLIGASI_OTHER_PARAMETER = "Obligasi\\OtherParameter\\OTHER_PARAMETER.xlsx";
        public const string OBLIGASI_REPORT_PARAMETER = "Obligasi\\ReportParameter\\REPORT_PARAMETER.xlsx";
        public const string OBLIGASI_SECURITIES_PARAMETER = "Obligasi\\SecuritiesParameter\\SECURITIES_PARAMETER.xlsx";

        //index
        public const string INDEX_MAINTENANCE = "Index\\Index Maintenance.xlsx";

        //Discount Levy
        public const string PELAPORAN_DISCOUNT_LEVY = "Equity\\PelaporanEvaluasi\\PerhitunganDiskon.xlsx";

        // pelaporan evaluasi
        public const string REKAPITULASI_INVALID_TRADING_ACCOUNT = "Equity\\PelaporanEvaluasi\\Rekapitulasi Invalid Trading Account.xlsx";
        public const string PERBANDINGAN_INVALID_TRADING_ACCOUNT = "Equity\\PelaporanEvaluasi\\Perbandingan Invalid Trading Account.xlsx";
        public const string PENCANTUMAN_INVALID_TRADING_ACCOUNT = "Equity\\PelaporanEvaluasi\\Pencantuman Invalid Trading Account.xlsx";
        public const string GRAFIK_INVALID_TRADING_ACCOUNT = "Equity\\PelaporanEvaluasi\\Grafik Invalid Trading Account.xlsx";
        public const string RAW_INVALID_TRADING_ACCOUNT = "Equity\\PelaporanEvaluasi\\Raw Invalid Trading Account.xlsx";
    }
}