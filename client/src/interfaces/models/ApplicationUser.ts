export default interface ApplicationUser {
	Id: string;
	Name: string;
	Email: string;
	IsEmailConfirm: boolean;
	Phones: string;
	Address: string;
	RoleId: number;
	IsDeleted: boolean;
	CreatedAt: Date;
	LastUpdated: Date;
}
