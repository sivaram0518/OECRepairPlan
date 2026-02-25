import React, { useState, useEffect } from "react";
import ReactSelect from "react-select";
import {
    assignUserToProcedure,
    removeUserFromProcedure,
    removeAllUsersFromProcedure,
} from "../../../api/api";
import { ASSIGN_USER_ERROR_MSG, REMOVE_ALL_USERS_ERROR_MSG, SELECT_USER_TO_ASSIGN } from '../../Constants/Constants';

const PlanProcedureItem = ({ planId, procedure, users, planProcedureUsers, setPlanProcedureUsers }) => {
    const [selectedUsers, setSelectedUsers] = useState([]);

    useEffect(() => {
        const assignedUsers = planProcedureUsers
            .filter((ppu) => ppu.planId === parseInt(planId) && ppu.procedureId === procedure.procedureId)
            .map((ppu) => ({
                label: ppu.user.name,
                value: ppu.user.userId,
            }));
        setSelectedUsers(assignedUsers);
    }, [planProcedureUsers, planId, procedure.procedureId]);

    const handleAssignUserToProcedure = async (selected) => {
        try {
            const currentUserIds = selectedUsers.map((u) => u.value);
            const newUserIds = selected ? selected.map((u) => u.value) : [];

            // Find users that were added and removed
            const addedUsers = newUserIds.filter((id) => !currentUserIds.includes(id));
            const removedUsers = currentUserIds.filter((id) => !newUserIds.includes(id));

            // Add new users
            for (const userId of addedUsers) {
                await assignUserToProcedure(planId, procedure.procedureId, userId);
            }

            // Remove users
            for (const userId of removedUsers) {
                await removeUserFromProcedure(planId, procedure.procedureId, userId);
            }

            setSelectedUsers(selected || []);

            if (setPlanProcedureUsers) {
                const updatedUsers = planProcedureUsers.filter(
                    (ppu) => !(ppu.planId === parseInt(planId) && ppu.procedureId === procedure.procedureId)
                );
                const newAssignments = (selected || []).map((user) => ({
                    planId: parseInt(planId),
                    procedureId: procedure.procedureId,
                    userId: user.value,
                    user: { userId: user.value, name: user.label },
                }));
                setPlanProcedureUsers([...updatedUsers, ...newAssignments]);
            }
        } catch (error) {
            console.error(ASSIGN_USER_ERROR_MSG + error);
        }
    };

    const handleRemoveAllUsers = async () => {
        try {
            await removeAllUsersFromProcedure(planId, procedure.procedureId);
            setSelectedUsers([]);
            if (setPlanProcedureUsers) {
                const updatedUsers = planProcedureUsers.filter(
                    (ppu) => !(ppu.planId === parseInt(planId) && ppu.procedureId === procedure.procedureId)
                );
                setPlanProcedureUsers(updatedUsers);
            }
        } catch (error) {
            console.error(REMOVE_ALL_USERS_ERROR_MSG + error);
        }
    };

    return (
        <div className="py-2">
            <div>
                <strong>{procedure.procedureTitle}</strong>
            </div>

            <div className="mt-2">
                <ReactSelect
                    className="mt-2"
                    placeholder={SELECT_USER_TO_ASSIGN}
                    isMulti={true}
                    options={users}
                    value={selectedUsers}
                    onChange={(e) => handleAssignUserToProcedure(e)}
                />
            </div>

            {selectedUsers.length > 0 && (
                <div className="mt-2">
                    <div className="mb-2">
                        <strong>Assigned Users:</strong>
                    </div>
                    <div className="d-flex flex-wrap gap-2">
                        {selectedUsers.map((user) => (
                            <span key={user.value} className="badge bg-info text-dark">
                                {user.label}
                                <button
                                    type="button"
                                    className="btn-close btn-close-white ms-2"
                                    onClick={() =>
                                        handleAssignUserToProcedure(
                                            selectedUsers.filter((u) => u.value !== user.value)
                                        )
                                    }
                                    style={{ fontSize: "0.75rem" }}
                                />
                            </span>
                        ))}
                    </div>
                    <button
                        className="btn btn-sm btn-danger mt-2"
                        onClick={handleRemoveAllUsers}
                    >
                        Remove All Users
                    </button>
                </div>
            )}
        </div>
    );
};

export default PlanProcedureItem;
