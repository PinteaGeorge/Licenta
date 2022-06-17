import { User } from "./user";

export class UserParams {
    gender: string;
    minAge = 18;
    maxAge = 99;
    pageNo = 1;
    pageSize = 10;
    orderBy: 'lastActive';

    constructor(user: User) {
        this.gender = user.gender === 'female' ? 'male' : 'female'
    }
}