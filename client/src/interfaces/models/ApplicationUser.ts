export default interface ApplicationUser {
	id: string;
	name: string;
	email: string;
	isEmailConfirmed: boolean;
	phones: string;
	address: string;
	roleId: number;
	isDeleted: boolean;
	createdAt: Date;
	lastUpdated: Date;
}
