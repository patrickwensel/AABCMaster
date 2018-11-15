/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TestData::
---------------------------------------------------------------------

	Cases test data
	
---------------------------------------------------------------------*/
 
SET NOCOUNT ON
 
SET IDENTITY_INSERT [dbo].[Cases] ON
GO
 
 
PRINT 'Inserting values into [Cases]'

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(383,'Mar 19 2016  2:10:33.5770000PM',DEFAULT,572,NULL,1,'AUTHORIZATION (HEALTHPLUS AMERIGROUP) IS TIED TO GITTY ENDZWEIG, BCBA
- Feb: 100 hours
- Jan: 96 hours
- March 15 Hours BCBA Michael Camhi and 104 Hours ABA Aid Shmuel Lamm
','Jan  1 2016 12:00:00.0000000AM',1012,NULL,'12-21-16 Vicki called - no answer',1,1,1,0,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(384,'Mar 19 2016  2:10:33.5830000PM',DEFAULT,573,NULL,0,'- 104 monthly ABA hour allowance
- Feb: 84 hours
- Jan: 80 hours
- March: 8 Hours Kari Frank Speech therapy, 20 hours BCA Gilli Rechany, 88 hours ABA Aide Karen Rechany
- April: 16 hours BCBA Gili Rechany 88 hours ABA Aide Karen Rechany','Jan 11 2016 12:00:00.0000000AM',1012,NULL,'12-21-16 Vicki called  - no answer/ left message',1,1,0,12,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(385,'Mar 19 2016  2:10:33.5900000PM',DEFAULT,574,NULL,-1,NULL,NULL,NULL,NULL,NULL,1,1,0,13,'The patient is deceased',NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(386,'Mar 19 2016  2:10:33.5930000PM',DEFAULT,575,NULL,0,'- 86 monthly ABA hour allowance
- Feb: 29 hours
- Jan: 49 hours
- March: 8 Hours ABA Aid Malka Izrailev and 20 Hours ABA Aid Raizy Izrailev
','Jan  1 2016 12:00:00.0000000AM',1013,NULL,NULL,1,1,1,8,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(387,'Mar 19 2016  2:10:33.6200000PM',DEFAULT,576,NULL,1,'AUTHORIZATION (VALUE OPTIONS) IS TIED TO BABATUNDE EFUNIYI, BCBA
- Feb: 82 hours
- Jan: 58 hours
- March: 86 hours
- April: 0 hours BCBA','Feb  1 2016 12:00:00.0000000AM',1013,'from 2:45 everyday in Hasc, also would like from 8:30-9:30 pm.',NULL,1,1,1,0,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(389,'Mar 19 2016  2:10:33.6330000PM',DEFAULT,578,NULL,1,'AUTHORIZATION (VALUE OPTIONS) IS TIED TO LANA BAVARO, BCBA
- Feb: 4 hours BCBA Miriam Levy 1 hour BCBA Randi Matsas - Lana Bavaro no ABA Aide hours
- Jan: 0 hours
- March: 1 Hurs BCBA Lana Schlactus-Bavaro and 19.5 Hours ABA Aid Chanie Goldman','Feb  1 2016 12:00:00.0000000AM',1013,NULL,NULL,1,1,1,0,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(390,'Mar 19 2016  2:10:33.6400000PM',DEFAULT,579,NULL,1,'- 103 monthly hour allowance
- Feb: 20.5 hours (Masha Auerbach)
- Jan: 65 = EXCEEDS FIDELIS (Nechy Fishman)
- March: 23.75 hours','Jan  1 2016 12:00:00.0000000AM',1013,'roizy karczag',NULL,1,1,1,0,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(391,'Mar 19 2016  2:10:33.6470000PM',DEFAULT,580,NULL,1,'- 86 monthly ABA hour allowance
- Feb: 33.25 hours
- Jan: 65 = Exceeds Fidelis Allowance!
- March: 43.25 hours','Jan  1 2016 12:00:00.0000000AM',1013,'seit',NULL,1,1,1,0,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(392,'Mar 19 2016  2:10:33.6530000PM',DEFAULT,581,NULL,1,'- 86 monthly hour allowance
- Feb: 45.75 hours (Hannah Kramer, Dobi Ehrlich)
- Jan: 34.5 hours (Hannah Kramer, Dobi Ehrlich)
- Mar: 53 hours (Hannah Kramer, Dobi Ehrlich)','Jan  1 2016 12:00:00.0000000AM',1013,'everyday in house from 9:00-2:30. would like s/o to come down to see what she can order',NULL,1,1,1,0,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(393,'Mar 19 2016  2:10:33.6600000PM',DEFAULT,582,NULL,-1,'- 0 monthly ABA hour allowance
- Feb: 0 hours
- Jan: 0 hours','Feb 15 2016 12:00:00.0000000AM',1019,'very low functioning female',NULL,0,0,1,11,'Has Health republic NJ .we are not iNN',NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(394,'Mar 19 2016  2:10:33.6630000PM',DEFAULT,583,NULL,0,'AUTHORIZATION (NY HEALTHGROUP) IS TIED TO LANA BAVARO, BCBA
- Feb: 84 hours
- Jan: 71 hours
- March: 85 hours','Jan  1 2016 12:00:00.0000000AM',1012,NULL,'12-21-16 Vicki called - no answer',1,1,0,4,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(395,'Mar 19 2016  2:10:33.6700000PM',DEFAULT,584,NULL,-1,'- 103 monthly ABA hour allowance
- Feb: 27.5 hours
- Jan: 10.5 hours
- March: 26.5 hours','Jan  1 2016 12:00:00.0000000AM',1013,'from 5:30 everyday. Will cancel p3 if has good therapist (6-7), 7-8 would also work','No Change',1,1,1,0,'no insurance',NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(396,'Mar 19 2016  2:10:33.6770000PM',DEFAULT,585,NULL,1,'AUTHORIZATION (VALUE OPTIONS) IS TIED TO GITTY ENDZWEIG
- Mar: 4.25 hours
- Feb: 3.5 hours
- Jan: 0 hours','Feb 16 2016 12:00:00.0000000AM',1013,'S: 4-6, M: 4-7:70, T, W, Th: 5-8, F:9-12, Sh: 9-12',NULL,1,1,1,0,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(398,'Mar 19 2016  2:10:33.6870000PM',DEFAULT,587,NULL,2,'AUTHORIZATION (VALUE OPTIONS) IS TIED TO GITTY ENDZWEIG, BCBA
- March: 38 hours
- Feb: 0 hours
- Jan: 0 hours
The split up for Geldaye Malik was approved. Following are the details:
Auth under Joseph Graus
10/1/2016-12/31/2016
','Feb 16 2016 12:00:00.0000000AM',1013,'Sat 6-8 / Sun 3:30-5:30 / Tues 6-7:30 / When school is closed','No Changes',1,1,1,0,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(400,'Mar 19 2016  2:10:33.7000000PM',DEFAULT,589,NULL,-1,'No authorization identified
- Feb: 0 hours
- Jan: 0 hours','Jan 18 2016 12:00:00.0000000AM',NULL,'          ON    HOLD  MOM',NULL,0,0,0,14,'parents dont want services',NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(401,'Mar 19 2016  2:10:33.7030000PM',DEFAULT,590,NULL,0,'- 105 monthly hour allowance
- Feb: 106 hours = EXCEEDS - 12 hrs Lana B. BCBA - 106 Hrs ABA Caryn Tawil
- Jan: 82.5 hours ABA Caryn Tawil - BCBA  14 hrs Lana B - Speech 5.25 Elisa Chrem
- March 5.25 Hours Speech Therapy Elisa Chrem - 13 hrs BCBA Lana B. - 105 ABA Caryn Tawil','Dec 23 2015 12:00:00.0000000AM',1012,NULL,'12-21-16 Vicki called spoke to mom all is good',1,1,0,4,NULL,1,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(402,'Mar 19 2016  2:10:33.7100000PM',DEFAULT,591,NULL,-1,'AUTHORIZATION (NY HEALTH GROUP) IS TIED TO MIRIAM LEVY, BCBA
- Feb: 30 hours
- Jan: 24 hours
- March: 1.5 Hours BCBA Miriam Levy and 39 Hours ABA Aid Samantha Francois','Jan  1 2016 12:00:00.0000000AM',1012,NULL,NULL,1,1,0,4,'Mom doesnt want services',NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(403,'Mar 19 2016  2:10:33.7130000PM',DEFAULT,592,NULL,1,'- 108 monthly ABA hour allowance
- Feb: 124.25 hours
- Jan: 145.75 hours','Feb  1 2016 12:00:00.0000000AM',1013,NULL,'12/28 Ruchy spoke to mother. starting services on Jan 1. she will update her therapists. joining sunday program',1,1,1,0,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(405,'Mar 19 2016  2:10:33.7230000PM',DEFAULT,594,NULL,1,'- 86 monthly ABA hour allowance
- Feb: 86 hours = Exceeds Fidelis Allowance!
- Jan: 76 hours = Exceeds Fidelis Allowance!
- March: 102 Hours ABA Aid','Jan  1 2016 12:00:00.0000000AM',1013,NULL,NULL,1,1,1,0,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(406,'Mar 19 2016  2:10:33.7300000PM',DEFAULT,595,NULL,-1,'- 104 monthly ABA hour allowance
- Feb: 17 hours
- Jan: 0 hours','Feb 15 2016 12:00:00.0000000AM',1013,'wants after 6 everyday and frid morn and Sunday after 5.',NULL,1,1,1,0,'Lost insurance',NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(408,'Mar 19 2016  2:10:33.7400000PM',DEFAULT,597,NULL,-1,'No hours found','Jan  1 2016 12:00:00.0000000AM',1012,NULL,NULL,1,1,0,5,'Parents dont want ABA',NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(409,'Mar 19 2016  2:10:33.7600000PM',DEFAULT,598,NULL,1,'- 105 monthly hour allowance
- Feb: 0 hours (No provider listed)
- Jan: 0 hours (No provider listed)','Mar  3 2016 12:00:00.0000000AM',1012,NULL,'12-22-16 Vicki called no answer- left message',1,1,1,0,NULL,1,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(410,'Mar 19 2016  2:10:33.7630000PM',DEFAULT,599,NULL,1,'Malky looking for ABA aide 9-27-16

AUTHORIZATION (VALUE OPTIONS) IS TIED TO SUSAN JORDAN, BCBA
- Feb: 0 hours
- Jan: 0 hours','Nov 15 2015 12:00:00.0000000AM',1021,NULL,'12-22-16 Vicki called, no answer left message',1,1,1,0,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(411,'Mar 19 2016  2:10:33.7700000PM',DEFAULT,600,NULL,1,'AUTHORIZATION (VALUE OPTIONS) IS TIED TO CHANAH KESSLER, BCBA
- Feb: 77.5 hours (Chaya Strasberg, Chaya Kaminetzky, Ana Tomsky,Miriam Yitzchakov)
- Jan: 20 hours (Chaya Strasberg)
- March: 16 Hours BCBA Chanah Kessler and 18.5 ABA Aid Chaya Strasberg 32 Hours ABA Aid Basha Bongart and 19.5 ABA Aid Moshe Strasberg','Jan  1 2016 12:00:00.0000000AM',1013,'Ana on Fri',NULL,1,1,1,0,NULL,1,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(412,'Mar 19 2016  2:10:33.7770000PM',DEFAULT,601,NULL,-1,'- 104 monthly ABA hour allowance
- Feb: 72 hours
- Jan: 95 hours
- March: 32 Hours ABA Aid Adrianna Fox and 12 Hours BCBA Lana Schlactus-Bavaro and 42 Hours ABA Aid Samantha Francois','Nov 24 2015 12:00:00.0000000AM',1012,NULL,NULL,1,1,1,0,'Mom thought he didn''t need ABA anymore',NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(413,'Mar 19 2016  2:10:33.7800000PM',DEFAULT,602,NULL,0,'AUTHORIZATION (VALUE OPTIONS) IS TIED TO GITTY ENDZWEIG, BCBA
- Feb: 32.25 hours
- Jan: 42.5 hours
- March: 24.5 hours','Oct 26 2015 12:00:00.0000000AM',1012,NULL,'12-22-16 Vicki Called no answer, left message',1,1,0,4,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(414,'Mar 19 2016  2:10:33.7830000PM',DEFAULT,603,NULL,1,'AUTHORIZATION (VALUE OPTIONS) IS TIED TO RACHEL GOULD, BCBA
- Feb: 23.5 hours
- Jan: 0 hours','Feb  1 2016 12:00:00.0000000AM',1013,'9 yr boy, sun- 9:00-3:00,shab- any time,  tues and thurs- 2:30-7:00',NULL,1,1,1,0,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(416,'Mar 19 2016  2:10:33.7970000PM',DEFAULT,605,NULL,0,'- 104 monthly ABA hour allowance
- Feb: 6.5 hours BCBA Celia Roche Ms ED BCBA
- Jan: 0 hours','Feb 15 2016 12:00:00.0000000AM',1012,'M-F  5 -8  S/S all day','12-22-16 Vicki Called, no answer left message',1,1,0,4,NULL,NULL,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(417,'Mar 19 2016  2:10:33.8000000PM',DEFAULT,606,NULL,1,'- 104 monthly ABA hour allowance
- Feb: 43 hours
- Jan: 0 hours
- March 86.5','Feb  1 2016 12:00:00.0000000AM',1013,NULL,NULL,1,1,1,0,NULL,4,0,0,NULL,NULL)
INSERT INTO [Cases] ([ID],[DateCreated],[rv],[PatientID],[CaseGeneratingReferralID],[CaseStatus],[CaseStatusNotes],[CaseStartDate],[CaseAssignedStaffID],[CaseRequiredHoursNotes],[CaseRequiredServicesNotes],[CaseHasPrescription],[CaseHasAssessment],[CaseHasIntake],[CaseStatusReason],[CaseDischargeNotes],[DefaultServiceLocationID],[CaseNeedsStaffing],[CaseNeedsRestaffing],[CaseRestaffingReason],[CaseRestaffReasonID])VALUES(418,'Mar 19 2016  2:10:33.8070000PM',DEFAULT,607,NULL,1,'- 104 hour monthly allowance
- Feb: 45.5 hours (Yitzchak D. Bender, Zissie Bender)
- Jan: 0 hours (Yitzchak D. Bender, Zissie Bender)
- March 26 Hours ABA Aid Yitzchak D. Bender and 18 Hours Zissie Bender','Feb  1 2016 12:00:00.0000000AM',1013,'frid- 11:45, shab- afternnon. Sun- morn . e/d after 5-7 . high functioning',NULL,1,1,1,0,'left agency',NULL,0,0,NULL,NULL)
PRINT 'Done'
 
 
SET IDENTITY_INSERT [dbo].[Cases] OFF
GO
SET NOCOUNT OFF
