CREATE TABLE [dbo].[dat_SortMain](
	[SortMainId] [int] IDENTITY(1,1) NOT NULL,
	[SharePointId] [int] NULL,
	[OstiId] [varchar](250) NULL,
	[OwnerEmployeeId] [varchar](6) NULL,
	[OwnerName] [varchar](100) NULL,
	[OwnerEmail] [varchar](100) NULL,
	[ReportNumber] [varchar](100) NULL,
	[ContractNumber] [varchar](100) NULL,
	[Status] [varchar](25) NULL,
	[OstiStatusMsg] [varchar](max) NULL,
	[ProductType] [varchar](25) NULL,
	[Title] [varchar](1024) NULL,
	[PublishTitle] [varchar](1024) NULL,
	[ReportNumbers] [varchar](1024) NULL,
	[PublicationDate] [date] NULL,
	[Language] [varchar](100) NULL,
	[Country] [varchar](100) NULL,
	[AccessLimitation] [varchar](50) NULL,
	[ReleasedDate] [date] NULL,
	[UrlInt] [varchar](1024) NULL,
	[ExemptionNumber] [varchar](100) NULL,
	[AccessReleaseDate] [datetime] NULL,
	[ConferenceName] [varchar](1024) NULL,
	[ConferenceLocation] [varchar](250) NULL,
	[ConferenceBeginDate] [date] NULL,
	[ConferenceEndDate] [date] NULL,
	[JournalType] [varchar](25) NULL,
	[JournalName] [varchar](1024) NULL,
	[JournalVolume] [varchar](100) NULL,
	[JournalIssue] [varchar](100) NULL,
	[JournalSerial] [varchar](100) NULL,
	[JournalStartPage] [int] NULL,
	[JournalEndPage] [int] NULL,
	[JournalDoi] [varchar](1024) NULL,
	[PublisherName] [varchar](1024) NULL,
	[PublisherCity] [varchar](255) NULL,
	[PublisherState] [varchar](100) NULL,
	[PublisherCountry] [varchar](100) NULL,
	[PatentAssignee] [varchar](1024) NULL,
	[Abstract] [varchar](max) NULL,
	[RelatedDocInfo] [varchar](max) NULL,
	[FurtherInfoContact] [varchar](1024) NULL,
	[ProductSize] [varchar](1024) NULL,
	[PublisherInfo] [varchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[ApprovedDate] [datetime] NULL,
	[DueDate] [datetime] NULL,
	[PublishDate] [datetime] NULL,
	[ReviewStatus] [varchar](50) NULL,
	[ReviewProgress] [int] NULL,
	[OstiStatus] [varchar](25) NULL,
	[OstiDate] [datetime] NULL,
	[DoiNum] [varchar](100) NULL,
	[StiSpId] [int] NULL,
	[OneYearReminderDate] [date] NULL,
	[OneYearReminderSent] [bit] NOT NULL CONSTRAINT [DF_dat_Sort_Main_OneYear]  DEFAULT ((0)),
	[StiNumber] [varchar](100) NULL,
	[Revision] [int] NULL,
	[ConferenceSponsor] [varchar](1024) NULL,
	[HasTechWriter] [bit] NOT NULL CONSTRAINT [DF_dat_SortMain_HasTechWriter]  DEFAULT ((0)),
	[TechWriterEmployeeId] [varchar](6) NULL,
	[CoverPageRequired] [bit] NOT NULL CONSTRAINT [DF_dat_SortMain_CoverPageRequired]  DEFAULT ((1)),
	[IsFromLrs] [bit] NOT NULL CONSTRAINT [DFdat_SortMain_IsFromLrs]  DEFAULT ((0)),
	[ForceEdms] [bit] NOT NULL CONSTRAINT [DF_dat_SortMain_ForceEdms]  DEFAULT ((0)),
	[RelatedSti] [varchar](250) NULL,
	[DelayToDate] [date] NULL,
	[DelayReason] [varchar](max) NULL,
	[DelayReminderSent] [bit] NOT NULL CONSTRAINT [DF_dat_SortMain_DelayReminderSent]  DEFAULT ((0)),
	CONSTRAINT [PK_dat_Osti] PRIMARY KEY CLUSTERED 
	(
		[SortMainId] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_dat_SortMain_Owner] ON [dbo].[dat_SortMain]
(
	[OwnerEmployeeId] ASC,
	[Status] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_dat_SortMain_Reminder] ON [dbo].[dat_SortMain]
(
	[OneYearReminderDate] ASC,
	[OneYearReminderSent] ASC,
	[Status] ASC
)
go

CREATE NONCLUSTERED INDEX [IX_dat_SortMain_SharePointId] ON [dbo].[dat_SortMain]
(
	[SharePointId] ASC
)
go

CREATE NONCLUSTERED INDEX [IX_dat_SortMain_Status] ON [dbo].[dat_SortMain]
(
	[Status] ASC
)
go

CREATE TABLE [dbo].[enum_UserRole](
	[Role] [varchar](50) NOT NULL,
	CONSTRAINT [PK_enum_Role] PRIMARY KEY CLUSTERED 
	(
		[Role] ASC
	)
)
go

insert into enum_UserRole values ('Admin')
insert into enum_UserRole values ('OrgManager')
insert into enum_UserRole values ('ReadAll')
insert into enum_UserRole values ('ReleaseOfficial')
insert into enum_UserRole values ('User')
go

CREATE TABLE [dbo].[dat_User](
	[EmployeeId] [varchar](6) NOT NULL,
	[Role] [varchar](50) NOT NULL,
	CONSTRAINT [PK_dat_User] PRIMARY KEY CLUSTERED 
	(
		[EmployeeId] ASC,
		[Role] ASC
	)
)
GO

ALTER TABLE [dbo].[dat_User] ADD  CONSTRAINT [FK_dat_User_enum_UserRole] FOREIGN KEY([Role])
REFERENCES [dbo].[enum_UserRole] ([Role])
GO

CREATE TABLE [dbo].[dat_UserOrg](
	[UserOrgId] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [varchar](6) NOT NULL,
	[Org] [varchar](5) NOT NULL,
	CONSTRAINT [PK_dat_UserOrg] PRIMARY KEY CLUSTERED 
	(
		[UserOrgId] ASC
	)
)
go

CREATE TABLE [dbo].[lu_CoreCapabilities](
	[CoreCapabilitiesId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](1024) NOT NULL,
	[Active] [bit] NOT NULL CONSTRAINT [DF_lu_CoreCapabilities_Active]  DEFAULT ((1)),
	CONSTRAINT [PK_lu_CoreCapabilities] PRIMARY KEY CLUSTERED 
	(
		[CoreCapabilitiesId] ASC
	)
)
GO

CREATE TABLE [dbo].[dat_Config](
	[ConfigKey] [nvarchar](100) NOT NULL,
	[ConfigValue] [nvarchar](max) NOT NULL,
	[Encrypted] [bit] NOT NULL CONSTRAINT [DF_dat_Config_Encrypted]  DEFAULT ((0)),
	CONSTRAINT [PK_dat_Config] PRIMARY KEY CLUSTERED 
	(
		[ConfigKey] ASC
	)
)
GO

insert into lu_CoreCapabilities ([Name]) values ('Accelerator Science and Technology')
insert into lu_CoreCapabilities ([Name]) values ('Advanced Computer Science, Visualization, and Data')
insert into lu_CoreCapabilities ([Name]) values ('Applied Materials Science and Engineering')
insert into lu_CoreCapabilities ([Name]) values ('Applied Mathematics')
insert into lu_CoreCapabilities ([Name]) values ('Biological and Bioprocess Engineering')
insert into lu_CoreCapabilities ([Name]) values ('Biological Systems Science')
insert into lu_CoreCapabilities ([Name]) values ('Chemical Engineering')
insert into lu_CoreCapabilities ([Name]) values ('Chemical and Molecular Science')
insert into lu_CoreCapabilities ([Name]) values ('Climate Change Science and Atmospheric Science')
insert into lu_CoreCapabilities ([Name]) values ('Computational Science')
insert into lu_CoreCapabilities ([Name]) values ('Condensed Matter Physics and Materials Science')
insert into lu_CoreCapabilities ([Name]) values ('Cyber and Information Sciences')
insert into lu_CoreCapabilities ([Name]) values ('Decision Science and Analysis')
insert into lu_CoreCapabilities ([Name]) values ('Earth Systems Science and Engineering')
insert into lu_CoreCapabilities ([Name]) values ('Environmental Subsurface Science')
insert into lu_CoreCapabilities ([Name]) values ('Large Scale User Facilities/Advanced Instrumentation')
insert into lu_CoreCapabilities ([Name]) values ('Mechanical Design and Engineering')
insert into lu_CoreCapabilities ([Name]) values ('Nuclear Engineering')
insert into lu_CoreCapabilities ([Name]) values ('Nuclear Physics')
insert into lu_CoreCapabilities ([Name]) values ('Nuclear and Radio Chemistry')
insert into lu_CoreCapabilities ([Name]) values ('Particle Physics')
insert into lu_CoreCapabilities ([Name]) values ('Plasma and Fusion Energy Science')
insert into lu_CoreCapabilities ([Name]) values ('Power Systems and Electrical Engineering')
insert into lu_CoreCapabilities ([Name]) values ('Systems Engineering and Integration')
GO

CREATE TABLE [dbo].[lu_Country](
	[CountryId] [int] IDENTITY(1,1) NOT NULL,
	[Country] [varchar](50) NOT NULL,
	[CountryCode] [varchar](2) NOT NULL,
	[Active] [bit] NOT NULL CONSTRAINT [DF_lu_Country_Active]  DEFAULT ((1)),
	CONSTRAINT [PK_lu_Country] PRIMARY KEY CLUSTERED 
	(
		[CountryId] ASC
	)
)
GO

insert into lu_Country (Country, CountryCode) values ('AAEC', 'XH')
insert into lu_Country (Country, CountryCode) values ('Afghanistan', 'AF')
insert into lu_Country (Country, CountryCode) values ('Albania', 'AL')
insert into lu_Country (Country, CountryCode) values ('Algeria', 'DZ')
insert into lu_Country (Country, CountryCode) values ('American Samoa', 'AS')
insert into lu_Country (Country, CountryCode) values ('Andorra', 'AD')
insert into lu_Country (Country, CountryCode) values ('Angola', 'AO')
insert into lu_Country (Country, CountryCode) values ('Anguilla', 'AI')
insert into lu_Country (Country, CountryCode) values ('Antarctica', 'AQ')
insert into lu_Country (Country, CountryCode) values ('Antigua and Barbuda', 'AG')
insert into lu_Country (Country, CountryCode) values ('Argentina', 'AR')
insert into lu_Country (Country, CountryCode) values ('Armenia', 'AM')
insert into lu_Country (Country, CountryCode) values ('Aruba', 'AW')
insert into lu_Country (Country, CountryCode) values ('Australia', 'AU')
insert into lu_Country (Country, CountryCode) values ('Austria', 'AT')
insert into lu_Country (Country, CountryCode) values ('Azerbaijan', 'AZ')
insert into lu_Country (Country, CountryCode) values ('Bahamas', 'BS')
insert into lu_Country (Country, CountryCode) values ('Bahrain', 'BH')
insert into lu_Country (Country, CountryCode) values ('Bangladesh', 'BD')
insert into lu_Country (Country, CountryCode) values ('Barbados', 'BB')
insert into lu_Country (Country, CountryCode) values ('Belarus', 'BY')
insert into lu_Country (Country, CountryCode) values ('Belgium', 'BE')
insert into lu_Country (Country, CountryCode) values ('Belize', 'BZ')
insert into lu_Country (Country, CountryCode) values ('Benin', 'BJ')
insert into lu_Country (Country, CountryCode) values ('Bermuda', 'BM')
insert into lu_Country (Country, CountryCode) values ('Bhutan', 'BT')
insert into lu_Country (Country, CountryCode) values ('Bolivia', 'BO')
insert into lu_Country (Country, CountryCode) values ('Bosnia and Herzegovina', 'BA')
insert into lu_Country (Country, CountryCode) values ('Botswana', 'BW')
insert into lu_Country (Country, CountryCode) values ('Bouvet Island', 'BV')
insert into lu_Country (Country, CountryCode) values ('Brazil', 'BR')
insert into lu_Country (Country, CountryCode) values ('British Indian Ocean Territory', 'IO')
insert into lu_Country (Country, CountryCode) values ('Brunei Darussalam', 'BN')
insert into lu_Country (Country, CountryCode) values ('Bulgaria', 'BG')
insert into lu_Country (Country, CountryCode) values ('Burkina Faso', 'BF')
insert into lu_Country (Country, CountryCode) values ('Burundi', 'BI')
insert into lu_Country (Country, CountryCode) values ('CEC', 'XE')
insert into lu_Country (Country, CountryCode) values ('CERN', 'XC')
insert into lu_Country (Country, CountryCode) values ('CMEA', 'XM')
insert into lu_Country (Country, CountryCode) values ('CTBTO', 'XQ')
insert into lu_Country (Country, CountryCode) values ('Cambodia', 'KH')
insert into lu_Country (Country, CountryCode) values ('Cameroon', 'CM')
insert into lu_Country (Country, CountryCode) values ('Canada', 'CA')
insert into lu_Country (Country, CountryCode) values ('Cape Verde', 'CV')
insert into lu_Country (Country, CountryCode) values ('Cayman Islands', 'KY')
insert into lu_Country (Country, CountryCode) values ('Central African Republic', 'CF')
insert into lu_Country (Country, CountryCode) values ('Chad', 'TD')
insert into lu_Country (Country, CountryCode) values ('Chile', 'CL')
insert into lu_Country (Country, CountryCode) values ('China', 'CN')
insert into lu_Country (Country, CountryCode) values ('Christmas Island', 'CX')
insert into lu_Country (Country, CountryCode) values ('Cocos (Keeling) Islands', 'CC')
insert into lu_Country (Country, CountryCode) values ('Colombia', 'CO')
insert into lu_Country (Country, CountryCode) values ('Comoros', 'KM')
insert into lu_Country (Country, CountryCode) values ('Congo', 'CG')
insert into lu_Country (Country, CountryCode) values ('Congo, The Democratic Republic of the', 'CD')
insert into lu_Country (Country, CountryCode) values ('Cook Islands', 'CK')
insert into lu_Country (Country, CountryCode) values ('Costa Rica', 'CR')
insert into lu_Country (Country, CountryCode) values ('Cote d''Ivoire', 'CI')
insert into lu_Country (Country, CountryCode) values ('Country unknown/Code not available', 'ZZ')
insert into lu_Country (Country, CountryCode) values ('Croatia', 'HR')
insert into lu_Country (Country, CountryCode) values ('Cuba', 'CU')
insert into lu_Country (Country, CountryCode) values ('Cyprus', 'CY')
insert into lu_Country (Country, CountryCode) values ('Czech Republic', 'CZ')
insert into lu_Country (Country, CountryCode) values ('Denmark', 'DK')
insert into lu_Country (Country, CountryCode) values ('Djibouti', 'DJ')
insert into lu_Country (Country, CountryCode) values ('Dominica', 'DM')
insert into lu_Country (Country, CountryCode) values ('Dominican Republic', 'DO')
insert into lu_Country (Country, CountryCode) values ('ESA', 'XZ')
insert into lu_Country (Country, CountryCode) values ('East Timor', 'TP')
insert into lu_Country (Country, CountryCode) values ('Ecuador', 'EC')
insert into lu_Country (Country, CountryCode) values ('Egypt', 'EG')
insert into lu_Country (Country, CountryCode) values ('El Salvador', 'SV')
insert into lu_Country (Country, CountryCode) values ('Equatorial Guinea', 'GQ')
insert into lu_Country (Country, CountryCode) values ('Eritrea', 'ER')
insert into lu_Country (Country, CountryCode) values ('Estonia', 'EE')
insert into lu_Country (Country, CountryCode) values ('Ethiopia', 'ET')
insert into lu_Country (Country, CountryCode) values ('FAO', 'XF')
insert into lu_Country (Country, CountryCode) values ('Falkland Islands (Malvinas)', 'FK')
insert into lu_Country (Country, CountryCode) values ('Faroe Islands', 'FO')
insert into lu_Country (Country, CountryCode) values ('Fiji', 'FJ')
insert into lu_Country (Country, CountryCode) values ('Finland', 'FI')
insert into lu_Country (Country, CountryCode) values ('France', 'FR')
insert into lu_Country (Country, CountryCode) values ('French Guiana', 'GF')
insert into lu_Country (Country, CountryCode) values ('French Polynesia', 'PF')
insert into lu_Country (Country, CountryCode) values ('French Southern Territories', 'TF')
insert into lu_Country (Country, CountryCode) values ('Gabon', 'GA')
insert into lu_Country (Country, CountryCode) values ('Gambia', 'GM')
insert into lu_Country (Country, CountryCode) values ('Georgia', 'GE')
insert into lu_Country (Country, CountryCode) values ('Germany', 'DE')
insert into lu_Country (Country, CountryCode) values ('Ghana', 'GH')
insert into lu_Country (Country, CountryCode) values ('Gibraltar', 'GI')
insert into lu_Country (Country, CountryCode) values ('Greece', 'GR')
insert into lu_Country (Country, CountryCode) values ('Greenland', 'GL')
insert into lu_Country (Country, CountryCode) values ('Grenada', 'GD')
insert into lu_Country (Country, CountryCode) values ('Guadeloupe', 'GP')
insert into lu_Country (Country, CountryCode) values ('Guam', 'GU')
insert into lu_Country (Country, CountryCode) values ('Guatemala', 'GT')
insert into lu_Country (Country, CountryCode) values ('Guinea', 'GN')
insert into lu_Country (Country, CountryCode) values ('Guinea-Bissau', 'GW')
insert into lu_Country (Country, CountryCode) values ('Guyana', 'GY')
insert into lu_Country (Country, CountryCode) values ('Haiti', 'HT')
insert into lu_Country (Country, CountryCode) values ('Heard and McDonald Islands', 'HM')
insert into lu_Country (Country, CountryCode) values ('Holy See, Vatican City State', 'VA')
insert into lu_Country (Country, CountryCode) values ('Honduras', 'HN')
insert into lu_Country (Country, CountryCode) values ('Hong Kong', 'HK')
insert into lu_Country (Country, CountryCode) values ('Hungary', 'HU')
insert into lu_Country (Country, CountryCode) values ('IAEA', 'XA')
insert into lu_Country (Country, CountryCode) values ('ICRP', 'XR')
insert into lu_Country (Country, CountryCode) values ('IEA', 'XY')
insert into lu_Country (Country, CountryCode) values ('IIASA', 'XI')
insert into lu_Country (Country, CountryCode) values ('ISO', 'XS')
insert into lu_Country (Country, CountryCode) values ('Iceland', 'IS')
insert into lu_Country (Country, CountryCode) values ('India', 'IN')
insert into lu_Country (Country, CountryCode) values ('Indonesia', 'ID')
insert into lu_Country (Country, CountryCode) values ('International organizations without location', 'AA')
insert into lu_Country (Country, CountryCode) values ('Iran, Islamic Republic of', 'IR')
insert into lu_Country (Country, CountryCode) values ('Iraq', 'IQ')
insert into lu_Country (Country, CountryCode) values ('Ireland', 'IE')
insert into lu_Country (Country, CountryCode) values ('Israel', 'IL')
insert into lu_Country (Country, CountryCode) values ('Italy', 'IT')
insert into lu_Country (Country, CountryCode) values ('JINR', 'XJ')
insert into lu_Country (Country, CountryCode) values ('Jamaica', 'JM')
insert into lu_Country (Country, CountryCode) values ('Japan', 'JP')
insert into lu_Country (Country, CountryCode) values ('Jordan', 'JO')
insert into lu_Country (Country, CountryCode) values ('Kazakstan (Kazakhstan)', 'KZ')
insert into lu_Country (Country, CountryCode) values ('Kenya', 'KE')
insert into lu_Country (Country, CountryCode) values ('Kiribati', 'KI')
insert into lu_Country (Country, CountryCode) values ('Korea, Democratic People''s Republic of', 'KP')
insert into lu_Country (Country, CountryCode) values ('Korea, Republic of', 'KR')
insert into lu_Country (Country, CountryCode) values ('Kuwait', 'KW')
insert into lu_Country (Country, CountryCode) values ('Kyrgyzstan', 'KG')
insert into lu_Country (Country, CountryCode) values ('Lao People''s Democratic Republic', 'LA')
insert into lu_Country (Country, CountryCode) values ('Latvia', 'LV')
insert into lu_Country (Country, CountryCode) values ('Lebanon', 'LB')
insert into lu_Country (Country, CountryCode) values ('Lesotho', 'LS')
insert into lu_Country (Country, CountryCode) values ('Liberia', 'LR')
insert into lu_Country (Country, CountryCode) values ('Libyan Arab Jamahiriya', 'LY')
insert into lu_Country (Country, CountryCode) values ('Liechtenstein', 'LI')
insert into lu_Country (Country, CountryCode) values ('Lithuania', 'LT')
insert into lu_Country (Country, CountryCode) values ('Luxembourg', 'LU')
insert into lu_Country (Country, CountryCode) values ('MERRAC', 'QQ')
insert into lu_Country (Country, CountryCode) values ('Macau', 'MO')
insert into lu_Country (Country, CountryCode) values ('Macedonia, The Former Yugoslav Republic of', 'MK')
insert into lu_Country (Country, CountryCode) values ('Madagascar', 'MG')
insert into lu_Country (Country, CountryCode) values ('Malawi', 'MW')
insert into lu_Country (Country, CountryCode) values ('Malaysia', 'MY')
insert into lu_Country (Country, CountryCode) values ('Maldives', 'MV')
insert into lu_Country (Country, CountryCode) values ('Mali', 'ML')
insert into lu_Country (Country, CountryCode) values ('Malta', 'MT')
insert into lu_Country (Country, CountryCode) values ('Marshall Islands', 'MH')
insert into lu_Country (Country, CountryCode) values ('Martinique', 'MQ')
insert into lu_Country (Country, CountryCode) values ('Mauritania', 'MR')
insert into lu_Country (Country, CountryCode) values ('Mauritius', 'MU')
insert into lu_Country (Country, CountryCode) values ('Mayotte', 'YT')
insert into lu_Country (Country, CountryCode) values ('Mexico', 'MX')
insert into lu_Country (Country, CountryCode) values ('Micronesia, Federated States of', 'FM')
insert into lu_Country (Country, CountryCode) values ('Moldova, Republic of', 'MD')
insert into lu_Country (Country, CountryCode) values ('Monaco', 'MC')
insert into lu_Country (Country, CountryCode) values ('Mongolia', 'MN')
insert into lu_Country (Country, CountryCode) values ('Montenegro', 'ME')
insert into lu_Country (Country, CountryCode) values ('Montserrat', 'MS')
insert into lu_Country (Country, CountryCode) values ('Morocco', 'MA')
insert into lu_Country (Country, CountryCode) values ('Mozambique', 'MZ')
insert into lu_Country (Country, CountryCode) values ('Myanmar', 'MM')
insert into lu_Country (Country, CountryCode) values ('NEA', 'XN')
insert into lu_Country (Country, CountryCode) values ('Namibia', 'NA')
insert into lu_Country (Country, CountryCode) values ('Nauru', 'NR')
insert into lu_Country (Country, CountryCode) values ('Nepal', 'NP')
insert into lu_Country (Country, CountryCode) values ('Netherlands', 'NL')
insert into lu_Country (Country, CountryCode) values ('Netherlands Antilles', 'AN')
insert into lu_Country (Country, CountryCode) values ('New Zealand', 'NZ')
insert into lu_Country (Country, CountryCode) values ('Nicaragua', 'NI')
insert into lu_Country (Country, CountryCode) values ('Niger', 'NE')
insert into lu_Country (Country, CountryCode) values ('Nigeria', 'NG')
insert into lu_Country (Country, CountryCode) values ('Niue', 'NU')
insert into lu_Country (Country, CountryCode) values ('Norfolk Island', 'NF')
insert into lu_Country (Country, CountryCode) values ('Northern Mariana Islands', 'MP')
insert into lu_Country (Country, CountryCode) values ('Norway', 'NO')
insert into lu_Country (Country, CountryCode) values ('OAU', 'XO')
insert into lu_Country (Country, CountryCode) values ('OECD', 'XD')
insert into lu_Country (Country, CountryCode) values ('Oman', 'OM')
insert into lu_Country (Country, CountryCode) values ('Pakistan', 'PK')
insert into lu_Country (Country, CountryCode) values ('Palau', 'PW')
insert into lu_Country (Country, CountryCode) values ('Palestinian Territory, Occupied', 'PS')
insert into lu_Country (Country, CountryCode) values ('Panama', 'PA')
insert into lu_Country (Country, CountryCode) values ('Papua New Guinea', 'PG')
insert into lu_Country (Country, CountryCode) values ('Paraguay', 'PY')
insert into lu_Country (Country, CountryCode) values ('Peru', 'PE')
insert into lu_Country (Country, CountryCode) values ('Philippines', 'PH')
insert into lu_Country (Country, CountryCode) values ('Pitcairn', 'PN')
insert into lu_Country (Country, CountryCode) values ('Poland', 'PL')
insert into lu_Country (Country, CountryCode) values ('Portugal', 'PT')
insert into lu_Country (Country, CountryCode) values ('Puerto Rico', 'PR')
insert into lu_Country (Country, CountryCode) values ('Qatar', 'QA')
insert into lu_Country (Country, CountryCode) values ('Reunion', 'RE')
insert into lu_Country (Country, CountryCode) values ('Romania', 'RO')
insert into lu_Country (Country, CountryCode) values ('Russian Federation', 'RU')
insert into lu_Country (Country, CountryCode) values ('Rwanda', 'RW')
insert into lu_Country (Country, CountryCode) values ('Saint Helena', 'SH')
insert into lu_Country (Country, CountryCode) values ('Saint Kitts and Nevis', 'KN')
insert into lu_Country (Country, CountryCode) values ('Saint Lucia', 'LC')
insert into lu_Country (Country, CountryCode) values ('Saint Vincent and the Grenadines', 'VC')
insert into lu_Country (Country, CountryCode) values ('Samoa', 'WS')
insert into lu_Country (Country, CountryCode) values ('San Marino', 'SM')
insert into lu_Country (Country, CountryCode) values ('Sao Tome and Principe', 'ST')
insert into lu_Country (Country, CountryCode) values ('Saudi Arabia', 'SA')
insert into lu_Country (Country, CountryCode) values ('Senegal', 'SN')
insert into lu_Country (Country, CountryCode) values ('Serbia', 'RS')
insert into lu_Country (Country, CountryCode) values ('Serbia and Montenegro', 'CS')
insert into lu_Country (Country, CountryCode) values ('Seychelles', 'SC')
insert into lu_Country (Country, CountryCode) values ('Sierra Leone', 'SL')
insert into lu_Country (Country, CountryCode) values ('Singapore', 'SG')
insert into lu_Country (Country, CountryCode) values ('Slovakia', 'SK')
insert into lu_Country (Country, CountryCode) values ('Slovenia', 'SI')
insert into lu_Country (Country, CountryCode) values ('Solomon Islands', 'SB')
insert into lu_Country (Country, CountryCode) values ('Somalia', 'SO')
insert into lu_Country (Country, CountryCode) values ('South Africa', 'ZA')
insert into lu_Country (Country, CountryCode) values ('South Georgia and the South Sandwich Islands', 'GS')
insert into lu_Country (Country, CountryCode) values ('Spain', 'ES')
insert into lu_Country (Country, CountryCode) values ('Sri Lanka', 'LK')
insert into lu_Country (Country, CountryCode) values ('St. Pierre and Miquelon', 'PM')
insert into lu_Country (Country, CountryCode) values ('Sudan', 'SD')
insert into lu_Country (Country, CountryCode) values ('Suriname', 'SR')
insert into lu_Country (Country, CountryCode) values ('Svalbard and Jan Mayen', 'SJ')
insert into lu_Country (Country, CountryCode) values ('Swaziland', 'SZ')
insert into lu_Country (Country, CountryCode) values ('Sweden', 'SE')
insert into lu_Country (Country, CountryCode) values ('Switzerland', 'CH')
insert into lu_Country (Country, CountryCode) values ('Syrian Arab Republic', 'SY')
insert into lu_Country (Country, CountryCode) values ('Taiwan, Province of China', 'TW')
insert into lu_Country (Country, CountryCode) values ('Tajikistan', 'TJ')
insert into lu_Country (Country, CountryCode) values ('Tanzania, United Republic of', 'TZ')
insert into lu_Country (Country, CountryCode) values ('Thailand', 'TH')
insert into lu_Country (Country, CountryCode) values ('Timor-Leste', 'TL')
insert into lu_Country (Country, CountryCode) values ('Togo', 'TG')
insert into lu_Country (Country, CountryCode) values ('Tokelau', 'TK')
insert into lu_Country (Country, CountryCode) values ('Tonga', 'TO')
insert into lu_Country (Country, CountryCode) values ('Trinidad and Tobago', 'TT')
insert into lu_Country (Country, CountryCode) values ('Tunisia', 'TN')
insert into lu_Country (Country, CountryCode) values ('Turkey', 'TR')
insert into lu_Country (Country, CountryCode) values ('Turkmenistan', 'TM')
insert into lu_Country (Country, CountryCode) values ('Turks and Caicos Islands', 'TC')
insert into lu_Country (Country, CountryCode) values ('Tuvalu', 'TV')
insert into lu_Country (Country, CountryCode) values ('UN', 'XU')
insert into lu_Country (Country, CountryCode) values ('UNIDO', 'XT')
insert into lu_Country (Country, CountryCode) values ('USSR', 'SU')
insert into lu_Country (Country, CountryCode) values ('Uganda', 'UG')
insert into lu_Country (Country, CountryCode) values ('Ukraine', 'UA')
insert into lu_Country (Country, CountryCode) values ('United Arab Emirates', 'AE')
insert into lu_Country (Country, CountryCode) values ('United Kingdom', 'GB')
insert into lu_Country (Country, CountryCode) values ('United States', 'US')
insert into lu_Country (Country, CountryCode) values ('United States Minor Outlying Islands', 'UM')
insert into lu_Country (Country, CountryCode) values ('Uruguay', 'UY')
insert into lu_Country (Country, CountryCode) values ('Uzbekistan', 'UZ')
insert into lu_Country (Country, CountryCode) values ('Vanuatu', 'VU')
insert into lu_Country (Country, CountryCode) values ('Venezuela', 'VE')
insert into lu_Country (Country, CountryCode) values ('Viet Nam', 'VN')
insert into lu_Country (Country, CountryCode) values ('Virgin Islands, British', 'VG')
insert into lu_Country (Country, CountryCode) values ('Virgin Islands, U.S.', 'VI')
insert into lu_Country (Country, CountryCode) values ('WEC', 'XX')
insert into lu_Country (Country, CountryCode) values ('WHO', 'XW')
insert into lu_Country (Country, CountryCode) values ('WMO', 'XK')
insert into lu_Country (Country, CountryCode) values ('Wallis and Futuna Islands', 'WF')
insert into lu_Country (Country, CountryCode) values ('Western Sahara', 'EH')
insert into lu_Country (Country, CountryCode) values ('Yemen', 'YE')
insert into lu_Country (Country, CountryCode) values ('Yugoslavia', 'YU')
insert into lu_Country (Country, CountryCode) values ('Zambia', 'ZM')
insert into lu_Country (Country, CountryCode) values ('Zimbabwe', 'ZW')
insert into lu_Country (Country, CountryCode) values ('Test', 'TE')
go

CREATE TABLE [dbo].[lu_DoeFundingCategory](
	[DoeFundingCategoryId] [int] IDENTITY(1,1) NOT NULL,
	[Category] [varchar](5) NOT NULL,
	[Description] [varchar](250) NOT NULL,
	[Active] [bit] NOT NULL CONSTRAINT [DF_lu_DoeFundingCategory_Active]  DEFAULT ((1)),
	[OstiRequiredDescription] [varchar](500) NULL,
	[CoverSheetDescription] [varchar](500) NULL,
	CONSTRAINT [PK_lu_FundingCategory] PRIMARY KEY CLUSTERED 
	(
		[DoeFundingCategoryId] ASC
	)
)
GO

insert into lu_DoeFundingCategory ([Category], [Description], [OstiRequiredDescription], [CoverSheetDescription]) values ('EE', 'Energy Efficiency/Renewable Energy (EE)', 'USDOE Office of Energy Efficiency and Renewable Energy (EERE)', 'Office of Energy Efficiency and Renewable Energy')
insert into lu_DoeFundingCategory ([Category], [Description], [OstiRequiredDescription], [CoverSheetDescription]) values ('EM', 'Environmental Management (EM)', 'USDOE Office of Environmental Management (EM)', 'Office of Environmental Management')
insert into lu_DoeFundingCategory ([Category], [Description], [OstiRequiredDescription], [CoverSheetDescription]) values ('FE', 'Fossil Energy (FE)', 'USDOE Office of Fossil Energy (FE)', 'Office of Fossil Energy')
insert into lu_DoeFundingCategory ([Category], [Description], [OstiRequiredDescription], [CoverSheetDescription]) values ('NE', 'Nuclear Energy (NE)', 'USDOE Office of Nuclear Energy (NE)', null)
insert into lu_DoeFundingCategory ([Category], [Description], [OstiRequiredDescription], [CoverSheetDescription]) values ('SC', 'Office of Science (SC)', 'USDOE Office of Science (SC)', null)
insert into lu_DoeFundingCategory ([Category], [Description], [OstiRequiredDescription], [CoverSheetDescription]) values ('EH', 'Environment/Safety/Health (EH)', 'USDOE Office of Environment, Safety and Health (EH)', 'Office of Environment, Safety and Health')
insert into lu_DoeFundingCategory ([Category], [Description], [OstiRequiredDescription], [CoverSheetDescription]) values ('RW', 'Civilian Radioactive Waste Mgmt (RW)', 'USDOE Office of Civilian Radioactive Waste Management (RW)', 'Office of Civilian Radioactive Waste Management')
insert into lu_DoeFundingCategory ([Category], [Description], [OstiRequiredDescription], [CoverSheetDescription]) values ('NA', 'National Nuclear Security Administration (NA)', 'USDOE National Nuclear Security Administration (NNSA)', 'National Nuclear Security Administration')
insert into lu_DoeFundingCategory ([Category], [Description], [OstiRequiredDescription], [CoverSheetDescription]) values ('NR', 'Naval Reactors (NR)', 'USDOE National Nuclear Security Administration (NNSA), Office of Naval Reactors (NA-30)', null)
insert into lu_DoeFundingCategory ([Category], [Description], [OstiRequiredDescription], [CoverSheetDescription]) values ('OE', 'Electricity Delivery and Energy Reliability (OE)', 'USDOE Office of Electricity Delivery and Energy Reliability (OE)', 'Office of Electricity Delivery and Energy Reliability')
insert into lu_DoeFundingCategory ([Category], [Description], [OstiRequiredDescription], [CoverSheetDescription]) values ('EIA', 'Energy Information Administration', 'USDOE Energy Information Administration (EIA)', 'Energy Information Administration')
insert into lu_DoeFundingCategory ([Category], [Description], [OstiRequiredDescription], [CoverSheetDescription]) values ('IN', 'Intelligence and Counterintelligence', 'USDOE Office of Intelligence and Counterintelligence (IN)', 'Office of Intelligence and Counterintelligence')
insert into lu_DoeFundingCategory ([Category], [Description], [OstiRequiredDescription], [CoverSheetDescription]) values ('CCTP', 'Climate Change Technology Program', 'USDOE Office of International Affairs (IA), Climate Change Policy and Technology (IA-40)', 'Office of International Affairs - Climate Change Policy and Technology')
go

CREATE TABLE [dbo].[lu_FundingType](
	[FundingTypeId] [int] IDENTITY(1,1) NOT NULL,
	[FundingType] [varchar](5) NOT NULL,
	[Description] [varchar](50) NOT NULL,
	[Active] [bit] NOT NULL CONSTRAINT [DF_lu_FundingType_Active]  DEFAULT ((1)),
	CONSTRAINT [PK_lu_FundingType] PRIMARY KEY CLUSTERED 
	(
		[FundingTypeId] ASC
	)
)
GO

insert into lu_FundingType (FundingType, [Description]) values ('DOE', 'Department Of Energy')
insert into lu_FundingType (FundingType, [Description]) values ('LDRD', 'Laboratory Directed Research and Development')
insert into lu_FundingType (FundingType, [Description]) values ('OTHER', 'Other')
insert into lu_FundingType (FundingType, [Description]) values ('SPP', 'Strategic Partnership Project')
insert into lu_FundingType (FundingType, [Description]) values ('Grant', 'Grant')
insert into lu_FundingType (FundingType, [Description]) values ('NEUP', 'Nuclear Energy University Program')
go

CREATE TABLE [dbo].[lu_Journal](
	[JournalId] [int] IDENTITY(1,1) NOT NULL,
	[JournalName] [varchar](1024) NOT NULL,
	[Active] [bit] NOT NULL CONSTRAINT [DF_lu_Journal_Active]  DEFAULT ((1)),
	CONSTRAINT [PK_lu_Journal] PRIMARY KEY CLUSTERED 
	(
		[JournalId] ASC
	)
)
GO

insert into lu_Journal ([JournalName]) values ('Analytical and Bioanalytical Chemistry')
insert into lu_Journal ([JournalName]) values ('Applied and Environmental Microbiology')
insert into lu_Journal ([JournalName]) values ('Applied Radiation and Isotopes')
insert into lu_Journal ([JournalName]) values ('Archives of Oral Biology')
insert into lu_Journal ([JournalName]) values ('ASME Journal of Energy Resources Technology')
insert into lu_Journal ([JournalName]) values ('ASME Journal of Engineering for Gas Turbines and Power')
insert into lu_Journal ([JournalName]) values ('Bioresource Technology')
insert into lu_Journal ([JournalName]) values ('Biotechnology and Bioengineering')
insert into lu_Journal ([JournalName]) values ('Catalysis Today')
insert into lu_Journal ([JournalName]) values ('Cellulose')
insert into lu_Journal ([JournalName]) values ('Chemical Geology')
insert into lu_Journal ([JournalName]) values ('Discrete Event Dynamic Systems Theory and Applications')
insert into lu_Journal ([JournalName]) values ('Environmental Microbiology')
insert into lu_Journal ([JournalName]) values ('Environmental Science and Technology')
insert into lu_Journal ([JournalName]) values ('Fusion and Engineering Design')
insert into lu_Journal ([JournalName]) values ('Fusion Science and Technology')
insert into lu_Journal ([JournalName]) values ('Geomicrobiological Journal')
insert into lu_Journal ([JournalName]) values ('Geophysics')
insert into lu_Journal ([JournalName]) values ('Hydrometallurgy')
insert into lu_Journal ([JournalName]) values ('Inorganic Chemistry')
insert into lu_Journal ([JournalName]) values ('International Journal of Astrobiology')
insert into lu_Journal ([JournalName]) values ('International Journal of Hydrogen Energy')
insert into lu_Journal ([JournalName]) values ('International Journal of Mass Spectrometry')
insert into lu_Journal ([JournalName]) values ('International Journal of Radiation Biology')
insert into lu_Journal ([JournalName]) values ('Journal of Analytical and Bioanalytical Chemistry')
insert into lu_Journal ([JournalName]) values ('Journal of Applied Electrochemistry')
insert into lu_Journal ([JournalName]) values ('Journal of Applied Physics')
insert into lu_Journal ([JournalName]) values ('Journal of Chemical Physics')
insert into lu_Journal ([JournalName]) values ('Journal of Computational Physics')
insert into lu_Journal ([JournalName]) values ('Journal of Contaminant Hydrology')
insert into lu_Journal ([JournalName]) values ('Journal of Engineering for Gas Turbines and Power')
insert into lu_Journal ([JournalName]) values ('Journal of Fuel Cell Science and Technology')
insert into lu_Journal ([JournalName]) values ('Journal of Membrane Science')
insert into lu_Journal ([JournalName]) values ('Journal of Nuclear Materials')
insert into lu_Journal ([JournalName]) values ('Journal of Phase Equilibria and Diffusion')
insert into lu_Journal ([JournalName]) values ('Journal of Physical Chemistry')
insert into lu_Journal ([JournalName]) values ('Journal of Physical Chemistry A')
insert into lu_Journal ([JournalName]) values ('Journal of Physics')
insert into lu_Journal ([JournalName]) values ('Journal of Radiation and Environmental Biophysics')
insert into lu_Journal ([JournalName]) values ('Journal of Radio analytical Chemistry')
insert into lu_Journal ([JournalName]) values ('Journal of Radioanalytical and Nuclear Chemistry')
insert into lu_Journal ([JournalName]) values ('Journal of Thermal Analysis and Calorimetry')
insert into lu_Journal ([JournalName]) values ('Materials and Design')
insert into lu_Journal ([JournalName]) values ('Materials Research Society Bulletin')
insert into lu_Journal ([JournalName]) values ('National Defense Journal')
insert into lu_Journal ([JournalName]) values ('Nature Materials')
insert into lu_Journal ([JournalName]) values ('Nuclear Engineering and Design')
insert into lu_Journal ([JournalName]) values ('Nuclear Fusion')
insert into lu_Journal ([JournalName]) values ('Nuclear Instrumental  Methods Physics Research B')
insert into lu_Journal ([JournalName]) values ('Nuclear Instruments and Methods in Physics Research')
insert into lu_Journal ([JournalName]) values ('Nuclear Physics A')
insert into lu_Journal ([JournalName]) values ('Nuclear Science and Technology')
insert into lu_Journal ([JournalName]) values ('Nuclear Technology')
insert into lu_Journal ([JournalName]) values ('Physica D; Nonlinear Phenomena')
insert into lu_Journal ([JournalName]) values ('Physical Chemistry and Chemical Physics')
insert into lu_Journal ([JournalName]) values ('Physical Review B')
insert into lu_Journal ([JournalName]) values ('Physical Review C')
insert into lu_Journal ([JournalName]) values ('Physical Review E')
insert into lu_Journal ([JournalName]) values ('Planetary and Space Science')
insert into lu_Journal ([JournalName]) values ('Proceedings of the National Academy of Science')
insert into lu_Journal ([JournalName]) values ('Proceedings of the Royal Society')
insert into lu_Journal ([JournalName]) values ('Radioachimica Acta')
insert into lu_Journal ([JournalName]) values ('Radwaste Solutions')
insert into lu_Journal ([JournalName]) values ('Reliability Engineering & System safety')
insert into lu_Journal ([JournalName]) values ('Sensing and Imaging')
insert into lu_Journal ([JournalName]) values ('Solvent Extraction and Ion Exchange')
insert into lu_Journal ([JournalName]) values ('Tectonics')
insert into lu_Journal ([JournalName]) values ('Water Resources Research')
insert into lu_Journal ([JournalName]) values ('Acta Materialia')
insert into lu_Journal ([JournalName]) values ('Separation Science and Technology')
insert into lu_Journal ([JournalName]) values ('Nuclear Science and Engineering')
insert into lu_Journal ([JournalName]) values ('Journal of Physical Chemistry B')
go

CREATE TABLE [dbo].[lu_Language](
	[LanguageId] [int] IDENTITY(1,1) NOT NULL,
	[Language] [varchar](50) NOT NULL,
	[Active] [bit] NOT NULL CONSTRAINT [DF_lu_Language_Active]  DEFAULT ((1)),
	CONSTRAINT [PK_lu_Language] PRIMARY KEY CLUSTERED 
	(
		[LanguageId] ASC
	)
)
GO

insert into lu_Language ([Language]) values ('Afrikaans')
insert into lu_Language ([Language]) values ('Albanian')
insert into lu_Language ([Language]) values ('Arabic')
insert into lu_Language ([Language]) values ('Armenian')
insert into lu_Language ([Language]) values ('Azerbaijani')
insert into lu_Language ([Language]) values ('Bulgarian')
insert into lu_Language ([Language]) values ('Burmese')
insert into lu_Language ([Language]) values ('Byelorussian')
insert into lu_Language ([Language]) values ('Chinese')
insert into lu_Language ([Language]) values ('Croatian')
insert into lu_Language ([Language]) values ('Czech')
insert into lu_Language ([Language]) values ('Danish')
insert into lu_Language ([Language]) values ('Dutch')
insert into lu_Language ([Language]) values ('English')
insert into lu_Language ([Language]) values ('Estonian')
insert into lu_Language ([Language]) values ('Finnish')
insert into lu_Language ([Language]) values ('Flemish')
insert into lu_Language ([Language]) values ('French')
insert into lu_Language ([Language]) values ('Georgian')
insert into lu_Language ([Language]) values ('German')
insert into lu_Language ([Language]) values ('Greek')
insert into lu_Language ([Language]) values ('Hebrew')
insert into lu_Language ([Language]) values ('Hindi')
insert into lu_Language ([Language]) values ('Hungarian')
insert into lu_Language ([Language]) values ('Indonesian')
insert into lu_Language ([Language]) values ('Iranian')
insert into lu_Language ([Language]) values ('Irish')
insert into lu_Language ([Language]) values ('Italian')
insert into lu_Language ([Language]) values ('Japanese')
insert into lu_Language ([Language]) values ('Kazakh')
insert into lu_Language ([Language]) values ('Kirghiz')
insert into lu_Language ([Language]) values ('Korean')
insert into lu_Language ([Language]) values ('Unknown')
insert into lu_Language ([Language]) values ('Latvian')
insert into lu_Language ([Language]) values ('Lithuanian')
insert into lu_Language ([Language]) values ('Macedonian')
insert into lu_Language ([Language]) values ('Malay')
insert into lu_Language ([Language]) values ('Moldovian')
insert into lu_Language ([Language]) values ('Mongolian')
insert into lu_Language ([Language]) values ('Norwegian')
insert into lu_Language ([Language]) values ('Persian')
insert into lu_Language ([Language]) values ('Polish')
insert into lu_Language ([Language]) values ('Portuguese')
insert into lu_Language ([Language]) values ('Romanian')
insert into lu_Language ([Language]) values ('Rumanian')
insert into lu_Language ([Language]) values ('Russian')
insert into lu_Language ([Language]) values ('Serbian')
insert into lu_Language ([Language]) values ('Serbo-Croat')
insert into lu_Language ([Language]) values ('Serbo-Croatian')
insert into lu_Language ([Language]) values ('Slovak')
insert into lu_Language ([Language]) values ('Slovene')
insert into lu_Language ([Language]) values ('Slovenian')
insert into lu_Language ([Language]) values ('Spanish')
insert into lu_Language ([Language]) values ('Swedish')
insert into lu_Language ([Language]) values ('Tajik')
insert into lu_Language ([Language]) values ('Thai')
insert into lu_Language ([Language]) values ('Turkish')
insert into lu_Language ([Language]) values ('Turkmen')
insert into lu_Language ([Language]) values ('Ukrainian')
insert into lu_Language ([Language]) values ('Uzbek')
insert into lu_Language ([Language]) values ('Vietnamese')
insert into lu_Language ([Language]) values ('Welsh')
insert into lu_Language ([Language]) values ('Other')
go

CREATE TABLE [dbo].[lu_SponsorOrg](
	[SponsorOrgId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](1024) NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[Active] [bit] NOT NULL CONSTRAINT [DF_lu_SponsorOrg_Active]  DEFAULT ((1)),
	CONSTRAINT [PK_lu_SponsorOrg] PRIMARY KEY CLUSTERED 
	(
		[SponsorOrgId] ASC
	)
)
GO

insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Advanced Research Projects Agency - Energy (ARPA-E)', 'ARPA-E')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Environment, Health, Safety and Security (AU)', 'AU')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Environment, Health, Safety and Security (AU), Office of Health and Safety (AU-10)', 'AU-10')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Environment, Health, Safety and Security (AU), Office of Environmental Protection, Sustainability Support and Analysis (AU-20)', 'AU-20')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Environment, Health, Safety and Security (AU), Office of Nuclear Safety (AU-30)', 'AU-30')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Environment, Health, Safety and Security (AU), Office of Security (AU-50)', 'AU-50')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Environment, Health, Safety and Security (AU), Office of Classification (AU-60)', 'AU-60')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Congressional and Intergovernmental Affairs (CI)', 'CI')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Congressional and Intergovernmental Affairs (CI), Energy Policy (CI-30)', 'CI-30')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Energy Efficiency and Renewable Energy (EERE)', 'EE')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Energy Efficiency and Renewable Energy (EERE), Bioenergy Technologies Office (EE-3B)', 'EE-3B')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Energy Efficiency and Renewable Energy (EERE), Fuel Cell Technologies Program (EE-3F)', 'EE-3F')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Energy Efficiency and Renewable Energy (EERE), Vehicle Technologies Office (EE-3V)', 'EE-3V')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Energy Efficiency and Renewable Energy (EERE), Geothermal Technologies Office (EE-4G)', 'EE-4G')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Energy Efficiency and Renewable Energy (EERE), Solar Energy Technologies Office (EE-4S)', 'EE-4S')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Energy Efficiency and Renewable Energy (EERE), Wind Energy Technologies Office (EE-4WE)', 'EE-4WE')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Energy Efficiency and Renewable Energy (EERE), Water Power Technologies Office (EE-4WP)', 'EE-4WP')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Energy Efficiency and Renewable Energy (EERE), Advanced Manufacturing Office (EE-5A)', 'EE-5A')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Energy Efficiency and Renewable Energy (EERE), Building Technologies Office (EE-5B)', 'EE-5B')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Energy Efficiency and Renewable Energy (EERE), Federal Energy Management Program Office (EE-5F)', 'EE-5F')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Energy Efficiency and Renewable Energy (EERE), Sustainability Performance Office (EE-5S)', 'EE-5S')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Energy Efficiency and Renewable Energy (EERE), Weatherization and Intergovernmental Program (EE-5W)', 'EE-5W')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Energy Information Administration (EIA)', 'EI')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Energy Information Administration (EIA), Office of Energy Statistics (EI-20)', 'EI-20')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Energy Information Administration (EIA), Office of Energy Analysis (EI-30)', 'EI-30')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Environmental Management (EM)', 'EM')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Environmental Management (EM), Office of Soil/Ground Water Remediation (EM-12)', 'EM-12')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Environmental Management (EM), Safety, Security and Quality Programs (EM-40)', 'EM-40')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Environmental Management (EM), Acquisition and Project Management (EM-50)', 'EM-50')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Fossil Energy (FE)', 'FE')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Fossil Energy (FE), Clean Coal and Carbon (FE-20)', 'FE-20')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Fossil Energy (FE), Oil and Natural Gas (FE-30)', 'FE-30')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Fossil Energy (FE), Petroleum Reserves (FE-40)', 'FE-40')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of General Counsel (GC)', 'GC')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of General Counsel (GC), Environmental and Nuclear Programs (GC-50)', 'GC-50')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of General Counsel (GC), Office of NEPA Policy and Compliance (GC-54)', 'GC-54')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of General Counsel (GC), Technology Transfer and Procurement (GC-60)', 'GC-60')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of General Counsel (GC), Energy Policy (GC-70)', 'GC-70')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of International Affairs (IA)', 'IA')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of International Affairs (IA), Climate Change Policy and Technology (IA-40)', 'IA-40')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE The Office of Indian Energy Policy and Programs (IE)', 'IE')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Inspector General (IG)', 'IG')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Inspector General (IG), Audits and Inspections (IG-30)', 'IG-30')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Chief Information Officer (IM)', 'IM')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Chief Information Officer (IM), IT Policy and Governance (IM-20)', 'IM-20')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Intelligence and Counterintelligence (IN)', 'IN')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Intelligence and Counterintelligence (IN), Office of Intelligence (IN-10)', 'IN-10')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Intelligence and Counterintelligence (IN), Office of Counterintelligence (IN-20)', 'IN-20')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Legacy Management (LM)', 'LM')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Legacy Management (LM), Office of Site Operations (LM-20)', 'LM-20')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE National Nuclear Security Administration (NNSA)', 'NA')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE National Nuclear Security Administration (NNSA), Office of Defense Programs (DP) (NA-10)', 'NA-10')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE National Nuclear Security Administration (NNSA), Office of Defense Nuclear Nonproliferation (NA-20)', 'NA-20')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE National Nuclear Security Administration (NNSA), Office of Fissile Materials Disposition (NA-26)', 'NA-26')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE National Nuclear Security Administration (NNSA), Office of Naval Reactors (NA-30)', 'NA-30')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE National Nuclear Security Administration (NNSA), Office of Emergency Operations (NA-40)', 'NA-40')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE National Nuclear Security Administration (NNSA), Office of Defense Nuclear Security (NA-70)', 'NA-70')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Nuclear Energy (NE)', 'NE')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Nuclear Energy (NE), Nuclear Facility Operations (NE-3)', 'NE-3')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Nuclear Energy (NE), Fuel Cycle Technologies (NE-5)', 'NE-5')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Nuclear Energy (NE), International Nuclear Energy Policy and Cooperation (NE-6)', 'NE-6')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Nuclear Energy (NE), Nuclear Reactor Technologies (NE-7)', 'NE-7')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Nuclear Energy (NE), Office of Space and Defense Power Systems (NE-75)', 'NE-75')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Electricity Delivery and Energy Reliability (OE)', 'OE')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Electricity Delivery and Energy Reliability (OE), Power Systems Engineering Research and Development (R&D) (OE-10)', 'OE-10')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Electricity Delivery and Energy Reliability (OE), Infrastructure Security and Energy Restoration (ISER) (OE-30)', 'OE-30')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Public Affairs (PA)', 'PA')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Public Affairs (PA), Public Information (PA-40)', 'PA-40')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Secretary of Energy (S)', 'S')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Science (SC)', 'SC')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Science (SC), Advanced Scientific Computing Research (ASCR) (SC-21)', 'SC-21')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Science (SC), Basic Energy Sciences (BES) (SC-22)', 'SC-22')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Science (SC), Biological and Environmental Research (BER) (SC-23)', 'SC-23')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Science (SC), Fusion Energy Sciences (FES) (SC-24)', 'SC-24')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Science (SC), High Energy Physics (HEP) (SC-25)', 'SC-25')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Science (SC), Nuclear Physics (NP) (SC-26)', 'SC-26')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Science (SC), Workforce Development for Teachers and Scientists (WDTS) (SC-27)', 'SC-27')
insert into lu_SponsorOrg ([Name], [Code]) values ('USDOE Office of Under Secretary for Science and Energy (US)', 'US')
go

CREATE TABLE [dbo].[lu_SppCategory](
	[SppCategoryId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryCode] [varchar](15) NOT NULL,
	[Category] [varchar](50) NOT NULL,
	[Active] [bit] NOT NULL CONSTRAINT [DF_lu_WfoFundingCategory_Active]  DEFAULT ((1)),
	CONSTRAINT [PK_lu_WFOCategory] PRIMARY KEY CLUSTERED 
	(
		[SppCategoryId] ASC
	)
)
GO

insert into lu_SppCategory ([CategoryCode], [Category]) values ('FEDERAL', 'Federal Agency')
insert into lu_SppCategory ([CategoryCode], [Category]) values ('STATE', 'State Agency')
insert into lu_SppCategory ([CategoryCode], [Category]) values ('IND', 'Industry')
insert into lu_SppCategory ([CategoryCode], [Category]) values ('LAB', 'Other Laboratory')
insert into lu_SppCategory ([CategoryCode], [Category]) values ('UNV', 'University')
insert into lu_SppCategory ([CategoryCode], [Category]) values ('Other', 'Other ')
insert into lu_SppCategory ([CategoryCode], [Category]) values ('Foreign', 'Foreign Institution')
go

CREATE TABLE [dbo].[lu_SppCategoryFederalAgency](
	[SppCategoryFederalAgencyId] [int] IDENTITY(1,1) NOT NULL,
	[FederalAgency] [varchar](50) NOT NULL,
	[Active] [bit] NOT NULL  CONSTRAINT [DF_lu_SppCategoryFederalAgency_Active]  DEFAULT ((1)),
	CONSTRAINT [PK_lu_SppCategoryFederalAgency] PRIMARY KEY CLUSTERED 
	(
		[SppCategoryFederalAgencyId] ASC
	)
)
GO

insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('NRC')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('Army')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('Air Force')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('Navy')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('Marines')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('Coast Guard')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('Dept. of Interior')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('Dept. of Justice')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('FAA')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('NASA')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('DARPA')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('EPA')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('Foreign Govt. Agency')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('IAEA')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('OECD')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('DIA')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('NIJ')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('ONR')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('HUD')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('FBI')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('Customs Service')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('Bureau of Rec.')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('Dept. of State')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('Homeland Sec.')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('Other')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('Dept. of Defense')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('DHS')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('NNSA')
insert into lu_SppCategoryFederalAgency ([FederalAgency]) values ('DTRA')
go

CREATE TABLE [dbo].[lu_State](
	[StateId] [int] IDENTITY(1,1) NOT NULL,
	[StateName] [varchar](50) NOT NULL,
	[ShortName] [varchar](3) NOT NULL,
	[Active] [bit] NOT NULL CONSTRAINT [DF_lu_State_Active]  DEFAULT ((1)),
	CONSTRAINT [PK_lu_State] PRIMARY KEY CLUSTERED 
	(
		[StateId] ASC
	)
)
GO

insert into lu_State ([StateName], [ShortName]) values ('Alabama', 'AL')
insert into lu_State ([StateName], [ShortName]) values ('Alaska', 'AK')
insert into lu_State ([StateName], [ShortName]) values ('Arizona', 'AZ')
insert into lu_State ([StateName], [ShortName]) values ('Arkansas', 'AR')
insert into lu_State ([StateName], [ShortName]) values ('California', 'CA')
insert into lu_State ([StateName], [ShortName]) values ('Colorado', 'CO')
insert into lu_State ([StateName], [ShortName]) values ('Connecticut', 'CT')
insert into lu_State ([StateName], [ShortName]) values ('Delaware', 'DE')
insert into lu_State ([StateName], [ShortName]) values ('Washington D.C.', 'DC')
insert into lu_State ([StateName], [ShortName]) values ('Florida', 'FL')
insert into lu_State ([StateName], [ShortName]) values ('Georgia', 'GA')
insert into lu_State ([StateName], [ShortName]) values ('Hawaii', 'HI')
insert into lu_State ([StateName], [ShortName]) values ('Idaho', 'ID')
insert into lu_State ([StateName], [ShortName]) values ('Illinois', 'IL')
insert into lu_State ([StateName], [ShortName]) values ('Indiana', 'IN')
insert into lu_State ([StateName], [ShortName]) values ('Iowa', 'IA')
insert into lu_State ([StateName], [ShortName]) values ('Kansas', 'KS')
insert into lu_State ([StateName], [ShortName]) values ('Kentucky', 'KY')
insert into lu_State ([StateName], [ShortName]) values ('Louisiana', 'LA')
insert into lu_State ([StateName], [ShortName]) values ('Maine', 'ME')
insert into lu_State ([StateName], [ShortName]) values ('Maryland', 'MD')
insert into lu_State ([StateName], [ShortName]) values ('Massachusetts', 'MA')
insert into lu_State ([StateName], [ShortName]) values ('Michigan', 'MI')
insert into lu_State ([StateName], [ShortName]) values ('Minnesota', 'MN')
insert into lu_State ([StateName], [ShortName]) values ('Mississippi', 'MS')
insert into lu_State ([StateName], [ShortName]) values ('Missouri', 'MO')
insert into lu_State ([StateName], [ShortName]) values ('Montana', 'MT')
insert into lu_State ([StateName], [ShortName]) values ('Nebraska', 'NE')
insert into lu_State ([StateName], [ShortName]) values ('Nevada', 'NV')
insert into lu_State ([StateName], [ShortName]) values ('New Hampshire', 'NH')
insert into lu_State ([StateName], [ShortName]) values ('New Jersey', 'NJ')
insert into lu_State ([StateName], [ShortName]) values ('New Mexico', 'NM')
insert into lu_State ([StateName], [ShortName]) values ('New York', 'NY')
insert into lu_State ([StateName], [ShortName]) values ('North Carolina', 'NC')
insert into lu_State ([StateName], [ShortName]) values ('North Dakota', 'ND')
insert into lu_State ([StateName], [ShortName]) values ('Ohio', 'OH')
insert into lu_State ([StateName], [ShortName]) values ('Oklahoma', 'OK')
insert into lu_State ([StateName], [ShortName]) values ('Oregon', 'OR')
insert into lu_State ([StateName], [ShortName]) values ('Pennsylvania', 'PA')
insert into lu_State ([StateName], [ShortName]) values ('Rhode Island', 'RI')
insert into lu_State ([StateName], [ShortName]) values ('South Carolina', 'SC')
insert into lu_State ([StateName], [ShortName]) values ('South Dakota', 'SD')
insert into lu_State ([StateName], [ShortName]) values ('Tennessee', 'TN')
insert into lu_State ([StateName], [ShortName]) values ('Texas', 'TX')
insert into lu_State ([StateName], [ShortName]) values ('Utah', 'UT')
insert into lu_State ([StateName], [ShortName]) values ('Vermont', 'VT')
insert into lu_State ([StateName], [ShortName]) values ('Virginia', 'VA')
insert into lu_State ([StateName], [ShortName]) values ('Washington', 'WA')
insert into lu_State ([StateName], [ShortName]) values ('West Virginia', 'WV')
insert into lu_State ([StateName], [ShortName]) values ('Wisconsin', 'WI')
insert into lu_State ([StateName], [ShortName]) values ('Wyoming', 'WY')
insert into lu_State ([StateName], [ShortName]) values ('Puerto Rico', 'PR')
insert into lu_State ([StateName], [ShortName]) values ('Virgin Islands', 'VI')
insert into lu_State ([StateName], [ShortName]) values ('American Samoa', 'AS')
insert into lu_State ([StateName], [ShortName]) values ('Guam', 'GU')
insert into lu_State ([StateName], [ShortName]) values ('Northern Mariana Islands', 'MP')
go

CREATE TABLE [dbo].[lu_SubjectCategory](
	[SubjectCategoryId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [varchar](5) NOT NULL,
	[Subject] [varchar](150) NOT NULL,
	[Active] [bit] NOT NULL CONSTRAINT [DF_lu_SubjectCategory_Active]  DEFAULT ((1)),
	CONSTRAINT [PK_lu_SubjectCategory] PRIMARY KEY CLUSTERED 
	(
		[SubjectCategoryId] ASC
	)
)
GO

insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('01', 'COAL, LIGNITE, AND PEAT')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('02', 'PETROLEUM')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('03', 'NATURAL GAS')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('04', 'OIL SHALES AND TAR SANDS')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('07', 'ISOTOPES AND RADIATION SOURCES')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('08', 'HYDROGEN')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('09', 'BIOMASS FUELS')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('10', 'SYNTHETIC FUELS')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('11', 'NUCLEAR FUEL CYCLE AND FUEL MATERIALS ')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('12', 'MGMT OF RADIOACTIVE AND NON-RADIOACTIVE WASTES FROM NUCLEAR FACILITIES')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('13', 'HYDRO ENERGY')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('14', 'SOLAR ENERGY')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('15', 'GEOTHERMAL ENERGY')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('16', 'TIDAL AND WAVE POWER')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('17', 'WIND ENERGY')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('20', 'FOSSIL-FUELED POWER PLANTS')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('21', 'SPECIFIC NUCLEAR REACTORS AND ASSOCIATED PLANTS')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('22', 'GENERAL STUDIES OF NUCLEAR REACTORS')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('24', 'POWER TRANSMISSION AND DISTRIBUTION')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('25', 'ENERGY STORAGE')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('29', 'ENERGY PLANNING, POLICY AND ECONOMY')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('30', 'DIRECT ENERGY CONVERSION')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('32', 'ENERGY CONSERVATION, CONSUMPTION, AND UTILIZATION')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('33', 'ADVANCED PROPULSION SYSTEMS')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('36', 'MATERIALS SCIENCE')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('37', 'INORGANIC, ORGANIC, PHYSICAL AND ANALYTICAL CHEMISTRY')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('38', 'RADIATION CHEMISTRY, RADIOCHEMISTRY, AND NUCLEAR CHEMISTRY')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('42', 'ENGINEERING')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('43', 'PARTICLE ACCELERATORS')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('45', 'MILITARY TECHNOLOGY, WEAPONRY, AND NATIONAL DEFENSE')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('46', 'INSTRUMENTATION RELATED TO NUCLEAR SCIENCE AND TECHNOLOGY')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('47', 'OTHER INSTRUMENTATION')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('54', 'ENVIRONMENTAL SCIENCES/GLOBAL CLIMATE CHANGE STUDIES AND CLIMATE MITIGATION')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('58', 'GEOSCIENCES')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('59', 'BASIC BIOLOGICAL SCIENCES/GENOMICS/GENOME RESEARCH')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('60', 'APPLIED LIFE SCIENCES')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('61', 'RADIATION PROTECTION AND DOSIMETRY')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('62', 'RADIOLOGY AND NUCLEAR MEDICINE')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('63', 'RADIATION, THERMAL, AND ENVIRON. POLLUT. EFFECTS ON LIVING ORGS.AND BIO. MTRLS')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('70', 'PLASMA PHYSICS AND FUSION TECHNOLOGY')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('71', 'CLASSICAL AND QUANTUM MECHANICS, GENERAL PHYSICS')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('72', 'PHYSICS OF ELEMENTARY PARTICLES AND FIELDS')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('73', 'NUCLEAR PHYSICS AND RADIATION PHYSICS')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('74', 'ATOMIC AND MOLECULAR PHYSICS')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('75', 'CONDENSED MATTER PHYSICS, SUPERCONDUCTIVITY AND SUPERFLUIDITY')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('77', 'NANOSCIENCE AND NANOTECHNOLOGY')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('79', 'ASTRONOMY AND ASTROPHYSICS')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('97', 'MATHEMATICS AND COMPUTING')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('98', 'NUCLEAR DISARMAMENT, SAFEGUARDS, AND PHYSICAL PROTECTION')
insert into lu_SubjectCategory ([CategoryId], [Subject]) values ('99', 'GENERAL AND MISCELLANEOUS')
go

CREATE TABLE [dbo].[dat_Author](
	[AuthorId] [int] IDENTITY(1,1) NOT NULL,
	[SortMainId] [int] NOT NULL,
	[SharePointId] [int] NULL,
	[FirstName] [varchar](250) NOT NULL,
	[MiddleName] [varchar](250) NULL,
	[LastName] [varchar](250) NULL,
	[Affiliation] [varchar](500) NULL,
	[Email] [varchar](250) NULL,
	[OrcidId] [varchar](250) NULL,
	[IsPrimary] [bit] NOT NULL CONSTRAINT [DF_dat_Author_Primary]  DEFAULT ((0)),
	[EmployeeId] [varchar](10) NULL,
	[AffiliationType] [varchar](25) NULL,
	[WorkOrg] [varchar](100) NULL,
	[CountryId] [int] NULL,
	[StateId] [int] NULL,
	CONSTRAINT [PK_dat_Author] PRIMARY KEY CLUSTERED 
	(
		[AuthorId] ASC
	)
)
GO

ALTER TABLE [dbo].[dat_Author]  ADD  CONSTRAINT [FK_dat_Author_SortMainId] FOREIGN KEY([SortMainId])
REFERENCES [dbo].[dat_SortMain] ([SortMainId])
GO

CREATE NONCLUSTERED INDEX [IX_dat_Author_SortMainId] ON [dbo].[dat_Author]
(
	[SortMainId] ASC
)
go

CREATE TABLE [dbo].[dat_Contact](
	[ContactId] [int] IDENTITY(1,1) NOT NULL,
	[SortMainId] [int] NOT NULL,
	[FirstName] [varchar](250) NOT NULL,
	[MiddleName] [varchar](250) NULL,
	[LastName] [varchar](250) NULL,
	[ContactType] [varchar](500) NULL,
	[Phone] [varchar](250) NULL,
	[EmployeeId] [varchar](10) NULL,
	[WorkOrg] [varchar](100) NULL,
	CONSTRAINT [PK_dat_Contact] PRIMARY KEY CLUSTERED 
	(
		[ContactId] ASC
	)
)
GO

ALTER TABLE [dbo].[dat_Contact] ADD  CONSTRAINT [FK_dat_Contact_SortMainId] FOREIGN KEY([SortMainId])
REFERENCES [dbo].[dat_SortMain] ([SortMainId])
GO

CREATE NONCLUSTERED INDEX [IX_dat_Contact_SortMainId] ON [dbo].[dat_Contact]
(
	[SortMainId] ASC
)
go

CREATE TABLE [dbo].[dat_EmailTemplate](
	[EmailTemplateId] [int] IDENTITY(1,1) NOT NULL,
	[EmailType] [varchar](50) NOT NULL,
	[Header] [varchar](max) NOT NULL,
	[Body] [varchar](max) NOT NULL,
	[EditedByEmployeeId] [varchar](6) NOT NULL,
	[LastEditDate] [datetime] NOT NULL,
	CONSTRAINT [PK_dat_EmailTemplate`] PRIMARY KEY CLUSTERED 
	(
		[EmailTemplateId] ASC
	)
)
go

CREATE TABLE [dbo].[dat_ErrorLog](
	[ErrorLogId] [int] IDENTITY(1,1) NOT NULL,
	[ErrorDate] [datetime] NOT NULL CONSTRAINT [DF_dat_ErrorLog_Date]  DEFAULT (getdate()),
	[SortmainId] [int] NULL,
	[Name] [varchar](100) NOT NULL,
	[Message] [varchar](max) NULL,
	[StackTrack] [varchar](max) NULL,
	CONSTRAINT [PK_dat_ErrorLog] PRIMARY KEY CLUSTERED 
	(
		[ErrorLogId] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_dat_ErrorLog_Date] ON [dbo].[dat_ErrorLog]
(
	[ErrorDate] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_dat_ErrorLog_SortMainId] ON [dbo].[dat_ErrorLog]
(
	[SortmainId] ASC
)
GO

CREATE TABLE [dbo].[dat_Funding](
	[FundingId] [int] IDENTITY(1,1) NOT NULL,
	[SortMainId] [int] NOT NULL,
	[FundingTypeId] [int] NOT NULL,
	[DoeFundingCategoryId] [int] NULL,
	[GrantNumber] [varchar](250) NULL,
	[Year] [varchar](4) NULL,
	[Org] [varchar](10) NULL,
	[Percent] [varchar](6) NULL,
	[ContractNumber] [varchar](250) NULL,
	[ProjectArea] [varchar](250) NULL,
	[TrackingNumber] [varchar](250) NULL,
	[ProjectNumber] [varchar](250) NULL,
	[PrincipalInvEmployeeId] [varchar](6) NULL,
	[SppCategoryId] [int] NULL,
	[FederalAgencyId] [int] NULL,
	[OtherDescription] [varchar](max) NULL,
	[CountryId] [int] NULL,
	[AdditionalInfo] [varchar](max) NULL,
	[SppApproved] [bit] NULL,
	[ApproveNoReason] [varchar](max) NULL,
	[MilestoneTrackingNumber] [varchar](250) NULL,
	CONSTRAINT [PK_dat_Funding] PRIMARY KEY CLUSTERED 
	(
		[FundingId] ASC
	)
)
GO

ALTER TABLE [dbo].[dat_Funding] ADD  CONSTRAINT [FK_dat_Funding_SortMainId] FOREIGN KEY([SortMainId])
REFERENCES [dbo].[dat_SortMain] ([SortMainId])
GO

CREATE NONCLUSTERED INDEX [IX_dat_Funding_SortMainId] ON [dbo].[dat_Funding]
(
	[SortMainId] ASC
)
go

CREATE TABLE [dbo].[dat_OpenNet](
	[OpenNetId] [int] IDENTITY(1,1) NOT NULL,
	[SortMainId] [int] NOT NULL,
	[AccessNumber] [varchar](250) NULL,
	[DocLocation] [varchar](1024) NULL,
	[FieldOfficeAym] [varchar](max) NULL,
	[DeclassificationStatus] [varchar](250) NULL,
	[DeclassificationDate] [date] NULL,
	[KeyWords] [varchar](max) NULL,
	CONSTRAINT [PK_dat_OpenNet] PRIMARY KEY CLUSTERED 
	(
		[OpenNetId] ASC
	)
)
GO

ALTER TABLE [dbo].[dat_OpenNet]  ADD  CONSTRAINT [FK_dat_OpenNet_SortMainId] FOREIGN KEY([SortMainId])
REFERENCES [dbo].[dat_SortMain] ([SortMainId])
GO

CREATE NONCLUSTERED INDEX [IX_dat_OpenNet_SortMainId] ON [dbo].[dat_OpenNet]
(
	[SortMainId] ASC
)
go

CREATE TABLE [dbo].[dat_OstiUpload](
	[OstiUploadId] [int] IDENTITY(1,1) NOT NULL,
	[SortMainId] [int] NOT NULL,
	[RecordDate] [datetime] NOT NULL CONSTRAINT [DF_dat_OstiUpload_RecordDate]  DEFAULT (getdate()),
	[RequestXml] [varchar](max) NULL,
	[Response] [varchar](max) NOT NULL,
	[Status] [varchar](255) NULL,
	CONSTRAINT [PK_dat_OstiUpload] PRIMARY KEY CLUSTERED 
	(
		[OstiUploadId] ASC
	)
)
GO

ALTER TABLE [dbo].[dat_OstiUpload]  WITH CHECK ADD  CONSTRAINT [FK_dat_OstiUpload_SortMainId] FOREIGN KEY([SortMainId])
REFERENCES [dbo].[dat_SortMain] ([SortMainId])
GO

CREATE NONCLUSTERED INDEX [IX_dat_OstiUpload_RecordDate] ON [dbo].[dat_OstiUpload]
(
	[RecordDate] ASC
)
go

CREATE NONCLUSTERED INDEX [IX_dat_OstiUpload_SortMainId] ON [dbo].[dat_OstiUpload]
(
	[SortMainId] ASC,
	[RecordDate] ASC
)
go

CREATE TABLE [dbo].[dat_ProcessLog](
	[ProcessLogId] [int] IDENTITY(1001,1) NOT NULL,
	[EntryDate] [datetime] NOT NULL CONSTRAINT [DF_dat_ProcessLog_EntryDate]  DEFAULT (getdate()),
	[SortMainId] [int] NULL,
	[Result] [varchar](100) NULL,
	[ResultDesc] [varchar](max) NULL,
	CONSTRAINT [pk_dat_Processing] PRIMARY KEY CLUSTERED 
	(
		[ProcessLogId] ASC
	)
)
GO

CREATE TABLE [dbo].[dat_ProtectedData](
	[ProtectedDataId] [int] IDENTITY(1,1) NOT NULL,
	[SortMainId] [int] NOT NULL,
	[Crada] [bit] NOT NULL CONSTRAINT [DF_dat_ProtectedDataCrada]  DEFAULT ((0)),
	[Description] [varchar](max) NULL,
	[ReleaseDate] [datetime] NULL,
	[ExemptNumber] [varchar](250) NULL,
	CONSTRAINT [PK_dat_ProtectedData] PRIMARY KEY CLUSTERED 
	(
		[ProtectedDataId] ASC
	)
)
GO

ALTER TABLE [dbo].[dat_ProtectedData]  ADD  CONSTRAINT [FK_dat_ProtectedData_SortMainId] FOREIGN KEY([SortMainId])
REFERENCES [dbo].[dat_SortMain] ([SortMainId])
GO

CREATE NONCLUSTERED INDEX [IX_dat_ProtectedData_SortMainId] ON [dbo].[dat_ProtectedData]
(
	[SortMainId] ASC
)
go

CREATE TABLE [dbo].[dat_Review](
	[ReviewId] [int] IDENTITY(1,1) NOT NULL,
	[SortMainId] [int] NOT NULL,
	[Reviewer] [varchar](1024) NOT NULL,
	[ReviewerType] [varchar](100) NOT NULL,
	[ReviewDate] [datetime] NULL,
	[Approval] [varchar](100) NULL,
	[Reason] [varchar](max) NULL,
	[Access] [varchar](250) NULL,
	[AccessReason] [varchar](max) NULL,
	[OfficialUse] [varchar](100) NULL,
	[Comments] [varchar](max) NULL,
	CONSTRAINT [PK_dat_Review] PRIMARY KEY CLUSTERED 
	(
		[ReviewId] ASC
	)
)
GO

ALTER TABLE [dbo].[dat_Review]  ADD  CONSTRAINT [FK_dat_Review_SortMainId] FOREIGN KEY([SortMainId])
REFERENCES [dbo].[dat_SortMain] ([SortMainId])
GO

CREATE NONCLUSTERED INDEX [IX_dat_Review_SortMainId] ON [dbo].[dat_Review]
(
	[SortMainId] ASC
)
GO

CREATE TABLE [dbo].[dat_SortAttachment](
	[SortAttachmentId] [int] IDENTITY(1,1) NOT NULL,
	[SortMainId] [int] NOT NULL,
	[UploadDate] [datetime] NOT NULL CONSTRAINT [DF_dat_SortAttachment_Date]  DEFAULT (getdate()),
	[UploadEmployeeId] [varchar](6) NOT NULL,
	[FileName] [varchar](250) NOT NULL,
	[Note] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedByEmployeeId] [varchar](6) NULL,
	[DeletedDate] [datetime] NULL,
	[AttachmentType] [varchar](25) NULL,
	[Size] [varchar](25) NOT NULL CONSTRAINT [DF_dat_SortAttachment_Size]  DEFAULT ('Unknown'),
	CONSTRAINT [PK_dat_SortAttachment] PRIMARY KEY CLUSTERED 
	(
		[SortAttachmentId] ASC
	)
)
GO

ALTER TABLE [dbo].[dat_SortAttachment]  ADD  CONSTRAINT [FK_dat_SortAttachment_SortMainId] FOREIGN KEY([SortMainId])
REFERENCES [dbo].[dat_SortMain] ([SortMainId])
GO

CREATE NONCLUSTERED INDEX [IX_dat_SortAttachment_SortMainId] ON [dbo].[dat_SortAttachment]
(
	[SortMainId] ASC
)
GO

CREATE TABLE [dbo].[dat_SortAttachmentFile](
	[SortAttachmentFileId] [int] IDENTITY(1,1) NOT NULL,
	[SortAttachmentId] [int] NULL,
	[Contents] [varbinary](max) NOT NULL,
	[PartNumber] [int] NOT NULL CONSTRAINT [DF_dat_SortAttachmentFile_PartNumber]  DEFAULT ((1)),
	[Size] [int] NOT NULL CONSTRAINT [DF_dat_SortAttachmentFile_Size]  DEFAULT ((0)),
	CONSTRAINT [PK_dat_SortAttachmentFile] PRIMARY KEY CLUSTERED 
	(
		[SortAttachmentFileId] ASC
	)
)
GO

ALTER TABLE [dbo].[dat_SortAttachmentFile] ADD  CONSTRAINT [FK_dat_SortAttachmentFile_SortAttachmentId] FOREIGN KEY([SortAttachmentId])
REFERENCES [dbo].[dat_SortAttachment] ([SortAttachmentId])
GO

CREATE NONCLUSTERED INDEX [IX_dat_SortAttachmentFile_SortAttachmentId] ON [dbo].[dat_SortAttachmentFile]
(
	[SortAttachmentId] ASC
)
go

CREATE TABLE [dbo].[dat_SortMetaData](
	[MetaDataId] [int] IDENTITY(1,1) NOT NULL,
	[SortMainId] [int] NOT NULL,
	[MetaDataType] [varchar](20) NOT NULL,
	[Data] [varchar](1024) NOT NULL,
	CONSTRAINT [PK_dat_SortMetaData] PRIMARY KEY CLUSTERED 
	(
		[MetaDataId] ASC
	)
)
GO

ALTER TABLE [dbo].[dat_SortMetaData]  ADD  CONSTRAINT [FK_dat_SortMetaData_SortMainId] FOREIGN KEY([SortMainId])
REFERENCES [dbo].[dat_SortMain] ([SortMainId])
GO

CREATE NONCLUSTERED INDEX [IX_dat_SortMetaData_SortMainId] ON [dbo].[dat_SortMetaData]
(
	[SortMainId] ASC,
	[MetaDataType] ASC
)
GO

CREATE VIEW [dbo].[vw_User]
AS
SELECT	
	u.EmployeeId, 
	u.Role, 
	'' as NameFull, /*Link to the database and tables that hold the employee info*/
	0 as UserID /*Link to the database and tables that hold the employee info*/
FROM	dat_User u
GO

CREATE FUNCTION [dbo].[Split]
(
    @String NVARCHAR(4000),
    @Delimiter NCHAR(1)
)
RETURNS TABLE
AS
RETURN
(
    WITH Split(stpos,endpos)
    AS(
        SELECT 0 AS stpos, CHARINDEX(@Delimiter,@String) AS endpos
        UNION ALL
        SELECT endpos+1, CHARINDEX(@Delimiter,@String,endpos+1)
            FROM Split
            WHERE endpos > 0
    )
    SELECT 'Id' = ROW_NUMBER() OVER (ORDER BY (SELECT 1)),
        'Data' = SUBSTRING(@String,stpos,COALESCE(NULLIF(endpos,0),LEN(@String)+1)-stpos)
    FROM Split
)
GO

CREATE procedure [dbo].[usp_SearchSorts](
	@DateOn int,
	@StartTime datetime,
	@EndTime datetime,
	@SearchContact bit,
	@SearchAuthor bit,
	@SearchFunding bit,
	@Status varchar(25) = null,
	@ProductType varchar(25) = null,
	@AccessLimitation varchar(25) = null,
	@Country varchar(100) = null,
	@Language varchar(100) = null,
	@PublishTitle varchar(1024) = null,
	@OwnerEmployeeId varchar(6) = null,
	@ReportNumbers varchar(1024) = null,
	@OstiId varchar(250) = null,
	@Subject varchar(1024) = null,
	@Keyword varchar(1024) = null,
	@ContactEmployeeId varchar(10) = null,
	@ContactType varchar(500) = null,
	@ContactWorkOrg varchar(4000) = null,
	@AuthorFName varchar(250) = null,
	@AuthorMName varchar(250) = null,
	@AuthorLName varchar(250) = null,
	@Affiliation varchar(500) = null,
	@OrcidId varchar(250) = null,
	@AuthorWorkOrg varchar(4000) = null,
	@AuthorCountryId varchar(4000) = null,
	@AuthorStateId varchar(4000) = null,
	@FiscalYear varchar(4) = null,
	@FundingTypeId int = null,
	@ContractNumber varchar(250) = null,
	@FundingOrg varchar(4000) = null,
	@DoeFundingCategoryId int = null,
	@MilestoneTrackingNumber varchar(250) = null,
	@GrantNumber varchar(250) = null,
	@TrackingNumber varchar(250) = null,
	@ProjectNumber varchar(250) = null,
	@PrincipalInvEmployeeId varchar(6) = null,
	@OtherDescription varchar(8000) = null,
	@SppApproved bit = null,
	@NotSppReason varchar(8000) = null,
	@SppCategoryId int = null,
	@FederalAgencyId int = null,
	@ForeignCountry int = null,
	@ForeignInfo varchar(8000) = null,
	@SearchPublisher bit = null,
	@PublisherName varchar(1024) = null,
	@PublisherCity varchar(255) = null,
	@PublisherState varchar(100) = null,
	@PublisherCountry varchar(100) = null,
	@SearchConference bit = null,
	@ConferenceName varchar(1024) = null,
	@ConferenceLocation varchar(250) = null,
	@ConferenceStart date = null,
	@ConferenceEnd date = null,
	@SearchJournal bit = null,
	@JournalName varchar(1024) = null,
	@JournalVolume varchar(100) = null,
	@JournalIssue varchar(100) = null,
	@JournalSerial varchar(100) = null,
	@JournalDoi varchar(1024) = null
)
as
BEGIN
	SET NOCOUNT ON
	
	SELECT DISTINCT s.SortMainId
	FROM dat_SortMain s
	LEFT JOIN dat_SortMetaData subdata on subdata.SortMainId = s.SortMainId
		and subdata.MetaDataType = 'SubjectCategories'
	LEFT JOIN dat_SortMetaData keyword on keyword.SortMainId = s.SortMainId
		and keyword.MetaDataType = 'Keywords'
	LEFT JOIN dat_Contact ct on ct.SortMainId = s.SortMainId
	LEFT JOIN dat_Author a on a.SortMainId = s.SortMainId
	LEFT JOIN dat_Funding f on f.SortMainId = s.SortMainId
	WHERE ( 
			 (@DateOn = 1 and CreateDate BETWEEN @StartTime and @EndTime)
		  or (@DateOn = 2 and OstiDate BETWEEN @StartTime and @EndTime)
		  or (@DateOn = 3 and ApprovedDate BETWEEN @StartTime and @EndTime)
		  or (@DateOn = 4 and ModifiedDate BETWEEN @StartTime and @EndTime)
		  or (@DateOn = 5 and ReleasedDate BETWEEN @StartTime and @EndTime)
		  or (@DateOn = 6 and PublicationDate BETWEEN @StartTime and @EndTime)
		  )
	AND (@Status is null or (@Status is not null and s.[Status] = @Status))
	AND (@ProductType is null or (@ProductType is not null and s.ProductType = @ProductType))
	AND (@AccessLimitation is null or (@AccessLimitation is not null and s.AccessLimitation = @AccessLimitation))
	AND (@Country is null or (@Country is not null and s.Country = @Country))
	AND (@Language is null or (@Language is not null and s.[Language] = @Language))
	AND (@PublishTitle is null or (@PublishTitle is not null and s.PublishTitle like ('%' + @PublishTitle + '%')))
	AND (@OwnerEmployeeId is null or (@OwnerEmployeeId is not null and s.OwnerEmployeeId = @OwnerEmployeeId))
	AND (@ReportNumbers is null or (@ReportNumbers is not null and s.ReportNumbers like ('%' + @ReportNumbers + '%')))
	AND (@OstiId is null or (@OstiId is not null and s.OstiId = @OstiId))
	AND (@Subject is null or (@Subject is not null and subdata.MetaDataId is not null and subdata.Data = @Subject))
	AND (@Keyword is null or (@Keyword is not null and keyword.MetaDataId is not null and keyword.Data like ('%' + @Keyword + '%')))
	AND (@SearchContact = 0 or (@SearchContact = 1 AND ct.ContactId IS NOT NULL
						   AND (@ContactEmployeeId is null or (@ContactEmployeeId is not null AND ct.EmployeeId = @ContactEmployeeId))
						   AND (@ContactType is null or (@ContactType is not null and ct.ContactType = @ContactType))
						   AND (@ContactWorkOrg is null or (@ContactWorkOrg is not null and ct.WorkOrg in (select [data] from dbo.Split(@ContactWorkOrg, ','))))
						  ))
	AND (@SearchAuthor = 0 or (@SearchAuthor = 1 AND a.AuthorId IS NOT NULL
						   AND (@Affiliation is null or (@Affiliation is not null AND a.Affiliation like ('%' + @Affiliation + '%')))
						   AND (@OrcidId is null or (@OrcidId is not null and a.OrcidId like ('%' + @OrcidId + '%')))
						   AND (@AuthorWorkOrg is null or (@AuthorWorkOrg is not null and a.WorkOrg in (select [data] from dbo.Split(@AuthorWorkOrg, ','))))
						   AND (@AuthorFName is null or (@AuthorFName is not null and (a.FirstName like ('%' + @AuthorFName + '%') or a.LastName like ('%' + @AuthorFName + '%'))))
						   AND (@AuthorMName is null or (@AuthorMName is not null and a.MiddleName like ('%' + @AuthorMName + '%')))
						   AND (@AuthorLName is null or (@AuthorLName is not null and (a.LastName like ('%' + @AuthorLName + '%') or a.FirstName like ('%' + @AuthorLName + '%'))))
						   AND (@AuthorCountryId is null or (@AuthorCountryId is not null and convert(varchar, a.CountryId) in (select [data] from dbo.Split(@AuthorCountryId, ','))))
						   AND (@AuthorStateId is null or (@AuthorStateId is not null and convert(varchar, a.StateId) in (select [data] from dbo.Split(@AuthorStateId, ','))))
						  ))
	AND (@SearchFunding = 0 or (@SearchFunding = 1 AND f.FundingId IS NOT NULL
						   AND (@FiscalYear is null or (@FiscalYear is not null AND f.[Year] = @FiscalYear))
						   AND (@FundingTypeId is null or (@FundingTypeId is not null and f.FundingTypeId = @FundingTypeId))
						   AND (@ContractNumber is null or (@ContractNumber is not null AND f.ContractNumber like ('%' + @ContractNumber + '%')))
						   AND (@FundingOrg is null or (@FundingOrg is not null AND f.Org in (select [data] from dbo.Split(@FundingOrg, ','))))
						   AND (@DoeFundingCategoryId is null or (@DoeFundingCategoryId is not null AND f.DoeFundingCategoryId = @DoeFundingCategoryId))
						   AND (@MilestoneTrackingNumber is null or (@MilestoneTrackingNumber is not null AND f.MilestoneTrackingNumber like ('%' + @MilestoneTrackingNumber + '%')))
						   AND (@GrantNumber is null or (@GrantNumber is not null AND f.GrantNumber like ('%' + @GrantNumber + '%')))
						   AND (@TrackingNumber is null or (@TrackingNumber is not null AND f.TrackingNumber like ('%' + @TrackingNumber + '%')))
						   AND (@ProjectNumber is null or (@ProjectNumber is not null AND f.ProjectNumber like ('%' + @ProjectNumber + '%')))
						   AND (@PrincipalInvEmployeeId is null or (@PrincipalInvEmployeeId is not null AND f.PrincipalInvEmployeeId = @PrincipalInvEmployeeId))
						   AND (@OtherDescription is null or (@OtherDescription is not null AND f.OtherDescription like ('%' + @OtherDescription + '%')))
						   AND (@SppApproved is null or (@SppApproved is not null AND f.SppApproved = @SppApproved))
						   AND (@NotSppReason is null or (@NotSppReason is not null AND f.ApproveNoReason like ('%' + @NotSppReason + '%')))
						   AND (@SppCategoryId is null or (@SppCategoryId is not null AND f.SppCategoryId = @SppCategoryId))
						   AND (@FederalAgencyId is null or (@FederalAgencyId is not null AND f.FederalAgencyId = @FederalAgencyId))
						   AND (@ForeignCountry is null or (@ForeignCountry is not null AND f.CountryId = @ForeignCountry))
						   AND (@ForeignInfo is null or (@ForeignInfo is not null AND f.AdditionalInfo like ('%' + @ForeignInfo + '%')))
						   
						  ))
	AND (@SearchPublisher is null or (@SearchPublisher is not null and @SearchPublisher = 1
						   AND (@PublisherName is null or (@PublisherName is not null AND s.PublisherName like ('%' + @PublisherName + '%')))
						   AND (@PublisherCity is null or (@PublisherCity is not null AND s.PublisherCity like ('%' + @PublisherCity + '%')))
						   AND (@PublisherState is null or (@PublisherState is not null AND s.PublisherState like ('%' + @PublisherState + '%')))
						   AND (@PublisherCountry is null or (@PublisherCountry is not null AND s.PublisherCountry = @PublisherCountry))
						  ))
	AND (@SearchConference is null or (@SearchConference is not null and @SearchConference = 1
						   AND (@ConferenceName is null or (@ConferenceName is not null AND s.ConferenceName like ('%' + @ConferenceName + '%')))
						   AND (@ConferenceLocation is null or (@ConferenceLocation is not null AND s.ConferenceLocation like ('%' + @ConferenceLocation + '%')))
						   AND (@ConferenceStart is null or (@ConferenceStart is not null AND s.ConferenceBeginDate >= @ConferenceStart))
						   AND (@ConferenceEnd is null or (@ConferenceEnd is not null AND s.ConferenceEndDate <= @ConferenceEnd))
						  ))
	AND (@SearchJournal is null or (@SearchJournal is not null and @SearchJournal = 1
						   AND (@JournalName is null or (@JournalName is not null AND s.JournalName like ('%' + @JournalName + '%')))
						   AND (@JournalVolume is null or (@JournalVolume is not null AND s.JournalVolume like ('%' + @JournalVolume + '%')))
						   AND (@JournalIssue is null or (@JournalIssue is not null AND s.JournalIssue like ('%' + @JournalIssue + '%')))
						   AND (@JournalSerial is null or (@JournalSerial is not null AND s.JournalSerial like ('%' + @JournalSerial + '%')))
						   AND (@JournalDoi is null or (@JournalDoi is not null AND s.JournalDoi like ('%' + @JournalDoi + '%')))
						  ))
END
GO

